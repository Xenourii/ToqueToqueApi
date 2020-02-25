using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ToqueToqueApi.Extensions;
using ToqueToqueApi.Infrastructures;

namespace ToqueToqueApi.Models
{
    public class FilterOptions<T, TEntity> : IValidatableObject
    {
        public string[] Filter { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new FilterOptionsProcessor<T, TEntity>(Filter);
            var validTerms = processor.GetValidTerms();
            var invalidTerms = processor.GetAllTerms()
                .ExceptBy(validTerms, v => v.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var term in invalidTerms)
            {
                if (term.InvalidSyntax)
                    yield return new ValidationResult(
                        $"Invalid filter syntax '{term.Name}'.",
                        new[] {nameof(Filter)});

                if (term.InvalidTerm)
                    yield return new ValidationResult(
                        $"Invalid filter term '{term.Name}'.",
                        new[] {nameof(Filter)});

                if (term.InvalidOperator)
                    yield return new ValidationResult(
                        $"Invalid filter operator '{term.Operator}'.",
                        new[] {nameof(Filter)});

                if (term.InvalidValue)
                    yield return new ValidationResult(
                        $"Invalid filter value '{term.Value}'.",
                        new[] {nameof(Filter)});
            }
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var processor = new FilterOptionsProcessor<T, TEntity>(Filter);
            return processor.Apply(query);
        }
    }
}