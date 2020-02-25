using System.Collections.Generic;
using ToqueToqueApi.Databases.Models;

namespace ToqueToqueApi.Services
{
    public interface IMealService
    {
        IEnumerable<MealDb> GetAll();
        MealDb GetById(int id);
        IEnumerable<MealDb> GetAllMealsFromUser(int ownerId);
        MealDb Create(MealDb meal);
        void Update(MealDb meal);
        void Delete(int id);
    }
}