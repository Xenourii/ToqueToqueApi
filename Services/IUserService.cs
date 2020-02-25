using System.Collections.Generic;
using ToqueToqueApi.Databases.Models;

namespace ToqueToqueApi.Services
{
    public interface IUserService
    {
        UserDb Authenticate(string email, string password);
        IEnumerable<UserDb> GetAll();
        UserDb Get(int id);
        UserDb Create(UserDb user, string password);
        void Update(UserDb user, string password = null);
        void Delete(int id);
    }
}