using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Claims;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;
using ToqueToqueApi.Extensions;
using ToqueToqueApi.Helpers;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Services
{
    public class SessionService : ISessionService
    {
        private readonly ToqueToqueContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mapper"></param>
        public SessionService(ToqueToqueContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public async Task<PagedResults<Session>> GetAll(
            PagingOptions pagingOptions,
            SortOptions<Session, SessionDb> sortOptions,
            FilterOptions<Session, SessionDb> filterOptions)
        {
            Debug.Assert(pagingOptions.Offset != null, "pagingOptions.Offset != null");
            Debug.Assert(pagingOptions.Limit != null, "pagingOptions.Limit != null");

            IQueryable<SessionDb> query = _dbContext.Sessions;
            query = filterOptions.Apply(query);
            query = sortOptions.Apply(query);
            query = query.Include(q => q.SessionMeals);
            var size = await query.CountAsync();

            var items = await query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value)
                .ToListAsync();

            return new PagedResults<Session>
            {
                Items = _mapper.Map<List<Session>>(items),
                TotalSize = size
            };
        }

        public async Task<PagedResults<Session>> GetAll(
            int userId,
            PagingOptions pagingOptions,
            SortOptions<Session, SessionDb> sortOptions,
            FilterOptions<Session, SessionDb> filterOptions,
            DistanceOptions distanceOptions)
        {
            var isSortingOptions = sortOptions.OrderBy != null;

            IQueryable<SessionDb> query = _dbContext.Sessions;
            query = filterOptions.Apply(query);
            query = sortOptions.Apply(query);
            query = query.Include(q => q.SessionMeals);
            var size = await query.CountAsync();

            var items = ApplyDistanceOptions(distanceOptions, query)
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value);

            var sessions = _mapper.Map<IList<Session>>(items);
            foreach (var session in sessions)
                session.BookingState = GetBookingState(session.Id, userId);

            FakeGeolocationWhenUserDoNotJoinSession(sessions);

            if (distanceOptions.IsGeolocationOptionSet)
                sessions = CalculateDistanceFromUser(sessions, distanceOptions);

            if (distanceOptions.IsDistanceOptionsSet && !isSortingOptions)
                sessions = sessions.OrderBy(s => s.DistanceInMeters).ToList();

            return new PagedResults<Session>
            {
                Items = sessions,
                TotalSize = size
            };
        }

        private static void FakeGeolocationWhenUserDoNotJoinSession(IEnumerable<Session> sessions)
        {
            var random = new Random();
            foreach (var session in sessions.Where(session => session.BookingState != 2))
                session.Geolocation = GeoDistanceHelper.FakeGeolocation(session.Geolocation,
                    random.Next(-65, 65), random.Next(-65, 65));
        }

        private IList<Session> CalculateDistanceFromUser(IList<Session> sessions, DistanceOptions distanceOptions)
        {
            var currentGeolocation = _mapper.Map<Geolocation>(distanceOptions);

            foreach (var session in sessions)
            {
                var distanceInKm = GeoDistanceHelper.DistanceBetween(_mapper.Map<Geolocation>(session.Geolocation),
                    currentGeolocation);
                session.DistanceInMeters = distanceInKm * 1000;
            }

            return sessions.ToList();
        }

        private IEnumerable<SessionDb> ApplyDistanceOptions(DistanceOptions distanceOptions, IQueryable<SessionDb> query)
        {
            var items = query.Include(i => i.Geolocation).ToList();
            if (!distanceOptions.IsDistanceOptionsSet)
                return items;

            var geolocation = _mapper.Map<Geolocation>(distanceOptions);
            return items.Where(s =>
                GeoDistanceHelper.DistanceBetween(_mapper.Map<Geolocation>(s.Geolocation), geolocation) <=
                distanceOptions.MaxDistanceInKm).ToList();
        }

        /// <summary>
        /// Récupère une session spécifique selon son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SessionDb GetById(int id) => _dbContext.Sessions.Include(s => s.BookingStateSessionUser).First(s => s.Id == id);


        /// <summary>
        /// Création d'une nouvelle session
        /// </summary>
        /// <param name="session">Les caractéristiques de la session à créer</param>
        /// <returns></returns>
        public SessionDb Create(SessionDb session)
        {
            // On n'autorise pas deux dates identiques pour un même utilisateur
            if (_dbContext.Sessions.Any(x => x.Creator == session.Creator && x.EventStarting == session.EventStarting))
                throw new SessionStartTimeDuplicateForUserException($"User already has a session at this time ('{session.EventStarting}').");

            _dbContext.Sessions.Add(session);
            _dbContext.SaveChanges();

            return session;
        }


        /// <summary>
        /// Suppression d'un plat
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var session = _dbContext.Sessions.Find(id);

            if (session == null)
                throw new NotFoundException("Session not found");

            _dbContext.Sessions.Remove(session);
            _dbContext.SaveChanges();
        }


        public void Join(int sessionId, ClaimsIdentity identity)
        {
            var session = _dbContext.Sessions.Include(x => x.BookingStateSessionUser).FirstOrDefault(x => x.Id == sessionId);

            if (session == null)
                throw new NotFoundException($"Session with id '{sessionId}' was not found.");

            if (identity == null)
                throw new NotAuthorizedException("No identity token found");

            var userId = JWTHelper.GetUserId(identity);

            if (session.BookingStateSessionUser == null)
                session.BookingStateSessionUser = new List<BookingStateSessionUserDb>();

            // On a trouvé la session, on vérifie que l'utilisateur ne soit pas déjà inscrit
            if (session.BookingStateSessionUser.Any(x => x.UserId == userId))
                throw new JoinSessionException("User has already joined this session.");

            // L'utilisateur qui a créé la session ne peut pas s'y inscrire
            if (session.Creator == userId)
                throw new JoinSessionException("User can not join his own session.");

            var bookingStateAcceptedId = _dbContext.BookingStates.First(x => x.Name == "Accepté").Id;

            // On vérifie que la session ne soit pas déjà pleine
            if (session.BookingStateSessionUser.Count(x => x.BookingStateId == bookingStateAcceptedId) >= session.AvailableTickets)
                throw new JoinSessionException("This session has no available places");

            // On inscrit l'utilisateur
            session.BookingStateSessionUser.Add(new BookingStateSessionUserDb
            {
                UserId = userId,
                BookingStateId = _dbContext.BookingStates.First(x => x.Name == "En attente").Id
            });

            // On vérifie si une conversation exite ou non
            var conversationsToCheck = _dbContext.Conversations.Include(c => c.ConversationUser).Where(c => c.Name == session.Title);
            var isConversationAlreadyCreated = false;
            foreach (var c in conversationsToCheck)
            {
                if (c.ConversationUser == null)
                    continue;

                foreach (var conversationUser in c.ConversationUser)
                {
                    if (conversationUser.UserId != userId)
                        continue;

                    isConversationAlreadyCreated = true;
                    break;
                }
            }

            if (!isConversationAlreadyCreated)
            {
                // On créé une conversation
                var conversation = new ConversationDb
                {
                    Name = session.Title
                };
                var userOwner = _dbContext.Users.Find(session.Creator);
                var userAdded = _dbContext.Users.Find(userId);
                conversation.ConversationUser = new List<ConversationUserDb>
                {
                    new ConversationUserDb
                    {
                        Conversation = conversation,
                        User = userOwner
                    },
                    new ConversationUserDb
                    {
                        Conversation = conversation,
                        User = userAdded
                    }
                };

                _dbContext.Conversations.Add(conversation);
            }
            
            _dbContext.SaveChanges();
        }


        public void Cancel(int id, ClaimsIdentity identity)
        {
            var session = _dbContext.Sessions.Include(x => x.BookingStateSessionUser).FirstOrDefault(x => x.Id == id);

            if (session == null)
                throw new NotFoundException($"Session with id '{id}' was not found.");

            if (identity == null)
                throw new NotAuthorizedException("No identity token found");


            var userId = JWTHelper.GetUserId(identity);

            if (session.BookingStateSessionUser == null)
                session.BookingStateSessionUser = new List<BookingStateSessionUserDb>();


            var bookingStateDeclinedId = _dbContext.BookingStates.First(x => x.Name == "Refusé").Id;


            // On a trouvé la session, on vérifie que l'utilisateur ne soit pas refusé (sinon il pourrait réessayer de rejoindre si on l'annule)
            if (session.BookingStateSessionUser.Any(x => x.UserId == userId && x.BookingStateId == bookingStateDeclinedId))
                throw new JoinSessionException("User was declined from this session.");


            // On vérifie aussi que l'utilisateur ai fait une demande pour rejoindre (afin d'éviter de renvoyer 200 quand rien ne change)
            if (!session.BookingStateSessionUser.Any(x => x.UserId == userId))
                throw new JoinSessionException("User is not waiting in this session.");

            // L'utilisateur qui a créé la session ne peut pas s'y inscrire
            if (session.Creator == userId)
                throw new JoinSessionException("User can not join his own session.");


            // On supprime l'inscription de l'utilisateur
            session.BookingStateSessionUser.RemoveAll(bsu => bsu.UserId == userId);

            _dbContext.SaveChanges();
        }


        public void AcceptUser(int sessionId, int userId, ClaimsIdentity identity)
        {
            var session = _dbContext.Sessions.Include(x => x.BookingStateSessionUser).FirstOrDefault(x => x.Id == sessionId);

            if (session == null)
                throw new NotFoundException($"Session with id '{sessionId}' was not found.");

            if (identity == null)
                throw new NotAuthorizedException("No identity token found");

            var sessionOwnerId = JWTHelper.GetUserId(identity);

            var bookingStateWaitingId = _dbContext.BookingStates.First(x => x.Name == "En attente").Id;
            var bookingStateAcceptedId = _dbContext.BookingStates.First(x => x.Name == "Accepté").Id;

            // Si l'utilisateur à accepter pour la session n'est pas trouvé "En attente"
            if (!session.BookingStateSessionUser.Any(x => x.SessionId == sessionId && x.UserId == userId && x.BookingStateId == bookingStateWaitingId))
                throw new UserNotWaitingForSessionException($"No userId '{userId}' found waiting for sessionId '{sessionId}'");

            session.BookingStateSessionUser.First(x => x.SessionId == sessionId && x.UserId == userId && x.BookingStateId == bookingStateWaitingId)
                .BookingStateId = bookingStateAcceptedId;


            _dbContext.SaveChanges();
        }


        public void DenyUser(int sessionId, int userId, ClaimsIdentity identity)
        {
            var session = _dbContext.Sessions.Include(x => x.BookingStateSessionUser).FirstOrDefault(x => x.Id == sessionId);

            if (session == null)
                throw new NotFoundException($"Session with id '{sessionId}' was not found.");

            if (identity == null)
                throw new NotAuthorizedException("No identity token found");

            var sessionOwnerId = JWTHelper.GetUserId(identity);

            var bookingStateWaitingId = _dbContext.BookingStates.First(x => x.Name == "En attente").Id;
            var bookingStateDenyId = _dbContext.BookingStates.First(x => x.Name == "Refusé").Id;

            // Si l'utilisateur à accepter pour la session n'est pas trouvé "En attente"
            if (!session.BookingStateSessionUser.Any(x => x.SessionId == sessionId && x.UserId == userId && x.BookingStateId == bookingStateWaitingId))
                throw new UserNotWaitingForSessionException($"No userId '{userId}' found waiting for sessionId '{sessionId}'");

            session.BookingStateSessionUser.First(x => x.SessionId == sessionId && x.UserId == userId && x.BookingStateId == bookingStateWaitingId)
                .BookingStateId = bookingStateDenyId;


            _dbContext.SaveChanges();
        }

        public int? GetBookingState(int sessionId, int userId)
        {
            int? bookingState = null;
            var session = _dbContext.Sessions.Include(s => s.BookingStateSessionUser).First(s => s.Id == sessionId);

            if (session?.BookingStateSessionUser == null)
                return null;

            foreach (var bookingStateSessionUserDb in session.BookingStateSessionUser)
                if (bookingStateSessionUserDb.SessionId == sessionId && bookingStateSessionUserDb.UserId == userId)
                {
                    bookingState = bookingStateSessionUserDb.BookingStateId;
                    break;
                }

            return bookingState;
        }
    }
}