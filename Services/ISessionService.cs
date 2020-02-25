using System.Threading.Tasks;
using System.Security.Claims;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models;

namespace ToqueToqueApi.Services
{
    public interface ISessionService
    {
        Task<PagedResults<Session>> GetAll(int userId, PagingOptions pagingOptions, SortOptions<Session, 
            SessionDb> sortOptions, FilterOptions<Session, SessionDb> filterOptions, DistanceOptions distanceOptions);

        Task<PagedResults<Session>> GetAll(PagingOptions pagingOptions, SortOptions<Session,
            SessionDb> sortOptions, FilterOptions<Session, SessionDb> filterOptions);

        SessionDb GetById(int id);
        SessionDb Create(SessionDb session);
        void Delete(int id);
        void Join(int sessionId, ClaimsIdentity identity);
        void Cancel(int id, ClaimsIdentity identity);
        void AcceptUser(int sessionId, int userId, ClaimsIdentity identity);
        void DenyUser(int sessionId, int userId, ClaimsIdentity identity);
        int? GetBookingState(int sessionId, int userId);
    }
}