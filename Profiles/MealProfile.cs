using System;
using System.Collections.Generic;
using AutoMapper;
using ToqueToqueApi.Databases;
using ToqueToqueApi.Databases.Models;
using ToqueToqueApi.Models.Meals;

namespace ToqueToqueApi.Profiles
{
    public class MealProfile : Profile
    {
        public MealProfile()
        {
            CreateMap<MealDb, Meal>()
                .ForMember(dest => dest.RealizationTimeInMinutes,
                    opt => opt.MapFrom(src => src.RealizationTime.TotalMinutes))
                .ForMember(dest => dest.Difficulty,
                    opt => opt.MapFrom(src => src.Difficulty.Id))
                .ForMember(dest => dest.Particularity,
                    opt => opt.MapFrom(src => src.Particularity.Id))
                .ForMember(dest => dest.Allergens,
                    opt => opt.MapFrom<AllergenResolver>());

            CreateMap<Meal, MealDb>()
                .ForMember(dest => dest.RealizationTime,
                    opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.RealizationTimeInMinutes)))
                .ForMember(dest => dest.Difficulty,
                    opt => opt.MapFrom<DifficultyDbResolver>())
                .ForMember(dest => dest.Particularity,
                    opt => opt.MapFrom<ParticularityDbResolver>())
                .ForMember(dest => dest.AllergenMeals,
                    opt => opt.MapFrom<AllergenDbResolver>());
        }
    }

    public class AllergenDbResolver : IValueResolver<Meal, MealDb, List<AllergenMealDb>>
    {
        private readonly ToqueToqueContext _dbContext;

        public AllergenDbResolver(ToqueToqueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<AllergenMealDb> Resolve(Meal source, MealDb destination, List<AllergenMealDb> destMember, ResolutionContext context)
        {
            var allergenMeals = new List<AllergenMealDb>();
            foreach (var allergenId in source.Allergens)
            {
                var allergenDb = _dbContext.Allergens.Find(allergenId);
                var allergenMeal = new AllergenMealDb
                {
                    Meal = destination,
                    Allergen = allergenDb
                };
                allergenMeals.Add(allergenMeal);
            }

            return allergenMeals;
        }
    }

    public class AllergenResolver : IValueResolver<MealDb, Meal, List<int>>
    {
        public List<int> Resolve(MealDb source, Meal destination, List<int> destMember, ResolutionContext context)
        {
            var allergens = new List<int>();
            if (source.AllergenMeals == null)
                return null;

            foreach (var allergenMealDb in source.AllergenMeals)
                allergens.Add(allergenMealDb.AllergenId);
            return allergens;
        }
    }

    public class DifficultyDbResolver : IValueResolver<Meal, MealDb, DifficultyDb>
    {
        private readonly ToqueToqueContext _dbContext;

        public DifficultyDbResolver(ToqueToqueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DifficultyDb Resolve(Meal source, MealDb destination, DifficultyDb destMember, ResolutionContext context)
            => _dbContext.Difficulties.Find(source.Difficulty);
    }

    public class ParticularityDbResolver : IValueResolver<Meal, MealDb, ParticularityDb>
    {
        private readonly ToqueToqueContext _dbContext;

        public ParticularityDbResolver(ToqueToqueContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ParticularityDb Resolve(Meal source, MealDb destination, ParticularityDb destMember, ResolutionContext context)
            => _dbContext.Particularities.Find(source.Particularity);
    }
}