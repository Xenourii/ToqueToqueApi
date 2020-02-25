using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;
using ToqueToqueApi.Models.Meals;
using Profile = AutoMapper.Profile;

namespace ToqueToqueApi.Profiles
{
    public class SessionProfile : Profile
    {
        public SessionProfile()
        {
            CreateMap<SessionDb, Session>()
                .ForMember(dest => dest.DistanceInMeters, opt => opt.Ignore())
                .ForMember(dest => dest.Meals, opt => opt.MapFrom<MealsResolver>())
                .ForMember(dest => dest.MealsId, opt => opt.MapFrom<MealsIdResolver>())
                .ForMember(dest => dest.CreatorProfile, opt => opt.MapFrom<CreatorProfileResolver>());
            CreateMap<Session, SessionDb>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.SessionMeals, opt => opt.MapFrom<SessionMealsDbResolver>());
        }
    }

    public class CreatorProfileResolver : IValueResolver<SessionDb, Session, Models.Profile>
    {
        private readonly ToqueToqueContext _dbContext;
        private readonly IMapper _mapper;

        public CreatorProfileResolver(ToqueToqueContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Models.Profile Resolve(SessionDb source, Session destination, Models.Profile destMember, ResolutionContext context)
        {
            var userIdCreator = source.Creator;
            var userDb = _dbContext.Users.Find(userIdCreator);

            if (userDb == null)
                return null;

            return _mapper.Map<Models.Profile>(userDb);
        }
    }

    public class SessionMealsDbResolver : IValueResolver<Session, SessionDb, List<SessionMealDb>>
    {
        private readonly ToqueToqueContext _dbContext;

        public SessionMealsDbResolver(ToqueToqueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SessionMealDb> Resolve(Session source, SessionDb destination, List<SessionMealDb> destMember, ResolutionContext context)
        {
            var sessionMeals = new List<SessionMealDb>();
            foreach (var mealId in source.MealsId)
            {
                var mealDb = _dbContext.Meals.Find(mealId);
                var sessionMeal = new SessionMealDb
                {
                    Meal = mealDb,
                    Session = destination
                };
                sessionMeals.Add(sessionMeal);
            }

            return sessionMeals;
        }
    }

    public class MealsIdResolver : IValueResolver<SessionDb, Session, List<int>>
    {
        public List<int> Resolve(SessionDb source, Session destination, List<int> destMember, ResolutionContext context)
        {
            var mealsId = new List<int>();
            if (source.SessionMeals == null)
                return null;

            foreach (var sessionMealDb in source.SessionMeals)
                mealsId.Add(sessionMealDb.MealId);

            return mealsId;
        }
    }

    public class MealsResolver : IValueResolver<SessionDb, Session, List<Meal>>
    {
        private readonly ToqueToqueContext _dbContext;
        private readonly IMapper _mapper;

        public MealsResolver(ToqueToqueContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public List<Meal> Resolve(SessionDb source, Session destination, List<Meal> destMember, ResolutionContext context)
        {
            var meals = new List<Meal>();
            if (source.SessionMeals == null)
                return null;

            foreach (var sessionMealDb in source.SessionMeals)
            {
                var mealDb = _dbContext.Meals
                    .Include(m => m.Difficulty)
                    .Include(m => m.Particularity)
                    .Include((m => m.AllergenMeals))
                    .First(s => s.Id == sessionMealDb.MealId);
                var meal = _mapper.Map<Meal>(mealDb);
                meals.Add(meal);
            }

            return meals;
        }
    }
}