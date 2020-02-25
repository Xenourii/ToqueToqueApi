using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Exceptions;

namespace ToqueToqueApi.Services
{
    public class MealService : IMealService
    {
        private readonly ToqueToqueContext _dbContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public MealService(ToqueToqueContext context)
        {
            _dbContext = context;
        }


        /// <summary>
        /// Récupère tous les plats
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MealDb> GetAll() => _dbContext.Meals
            .Include(m => m.Difficulty)
            .Include(m => m.Particularity)
            .Include(m => m.AllergenMeals);


        /// <summary>
        /// Récupère un plat spécifique selon son id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MealDb GetById(int id) => _dbContext.Meals.Find(id);


        /// <summary>
        /// Récupère les plats d'un utilisateur
        /// </summary>
        public IEnumerable<MealDb> GetAllMealsFromUser(int ownerId) => _dbContext.Meals.Where(x => x.OwnerId == ownerId);


        /// <summary>
        /// Création d'un nouveau plat
        /// </summary>
        /// <param name="meal">Les caractéristiques du plat à créer</param>
        /// <returns></returns>
        public MealDb Create(MealDb meal)
        {
            // On n'autorise pas deux même titres de plat pour un même utilisateur
            if (_dbContext.Meals.Any(x => x.OwnerId == meal.OwnerId && x.Title == meal.Title))
                throw new MealTitleDuplicateForUserException($"User already has a meal named '{meal.Title}'.");

            _dbContext.Meals.Add(meal);
            _dbContext.SaveChanges();

            return meal;
        }


        /// <summary>
        /// Modifier un plat
        /// </summary>
        /// <param name="meal">Plat à modifier (l'id doit être le même que le précédent)</param>
        public void Update(MealDb meal)
        {
            // Si on ne trouve aucun plat avec l'id correspondant on ne fait pas d'update
            if (!_dbContext.Meals.Any(x => x.Id == meal.Id))
                throw new MealIdNotFoundException($"Meal id '{meal.Id}' was not found.");


            var myMeal = _dbContext.Meals.Find(meal.Id);

            // On vérifie que le titre n'existe pas déjà pris pour cet utilisateur
            if (!string.IsNullOrWhiteSpace(meal.Title) && !_dbContext.Meals.Any(x => x.OwnerId == meal.OwnerId && x.Title == meal.Title))
                throw new MealTitleDuplicateForUserException($"User already has a meal named '{meal.Title}'.");

            // update user properties if provided
            if (meal.AllergenMeals != null)
                myMeal.AllergenMeals = meal.AllergenMeals;

            if (!string.IsNullOrEmpty(meal.Description))
                myMeal.Description = meal.Description;

            if (meal.Difficulty != null)
                myMeal.Difficulty = meal.Difficulty;

            if (!string.IsNullOrEmpty(meal.LinkToFullMeal))
                myMeal.LinkToFullMeal = meal.LinkToFullMeal;

            if (meal.Particularity != null)
                myMeal.Particularity = meal.Particularity;

            if (meal.Pictures != null)
                myMeal.Pictures = meal.Pictures;

            if (meal.RealizationTime != null && meal.RealizationTime > TimeSpan.Zero)
                myMeal.RealizationTime = meal.RealizationTime;

            if (meal.SessionMeals != null)
                myMeal.SessionMeals = meal.SessionMeals;

            if (!string.IsNullOrEmpty(meal.Title))
                myMeal.Title = meal.Title;


            _dbContext.Meals.Update(meal);
            _dbContext.SaveChanges();
        }



        /// <summary>
        /// Suppression d'un plat
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var meal = _dbContext.Meals.Find(id);

            if (meal == null)
                throw new NotFoundException("Meal not found");

            _dbContext.Meals.Remove(meal);
            _dbContext.SaveChanges();
        }
    }
}