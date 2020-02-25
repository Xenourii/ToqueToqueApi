using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToqueToqueApi.Infrastructures
{
    public class FilterOptionsProcessor<T, TEntity>
    {
        private readonly string[] _filterQuery;

        public FilterOptionsProcessor(string[] filterQuery)
        {
            _filterQuery = filterQuery;
        }

        public IEnumerable<FilterTerm> GetAllTerms()
        {
            if (_filterQuery == null) yield break;

            var declaredTerms = GetTermsFromModel();

            string termName;
            string termOperator;
            string termValue;

            foreach (var expression in _filterQuery)
            {
                if (string.IsNullOrEmpty(expression)) continue;

                var tokens = expression.Split(' ');

                if (tokens.Length == 0)
                {
                    yield return new FilterTerm
                    {
                        InvalidSyntax = true,
                        Name = expression
                    };

                    continue;
                }

                termName = tokens[0];
                if (tokens.Length < 3)
                {
                    yield return new FilterTerm
                    {
                        InvalidSyntax = true,
                        Name = expression
                    };

                    continue;
                }

                var declaredTerm = GetDeclaredTerm(declaredTerms, termName);
                if (declaredTerm == null)
                {
                    yield return new FilterTerm
                    {
                        InvalidTerm = true,
                        Name = termName
                    };

                    continue;
                }

                termOperator = tokens[1];
                if (!declaredTerm.ExpressionProvider.GetOperators().Contains(termOperator))
                {
                    yield return new FilterTerm
                    {
                        InvalidOperator = true,
                        Operator = termOperator
                    };

                    continue;
                }

                termValue = string.Join(' ', tokens.Skip(2));
                if (!declaredTerm.ExpressionProvider.IsValidValue(termValue))
                {
                    yield return new FilterTerm
                    {
                        InvalidValue = true,
                        Value = termValue
                    };

                    continue;
                }

                yield return new FilterTerm
                {
                    Name = termName,
                    Operator = termOperator,
                    Value = termValue
                };
            }
        }

        public IEnumerable<FilterTerm> GetValidTerms()
        {
            var queryTerms = GetAllTerms()
                .Where(x => !x.InvalidSyntax &&
                            !x.InvalidTerm &&
                            !x.InvalidOperator &&
                            !x.InvalidValue)
                .ToArray();

            if (!queryTerms.Any()) yield break;

            var declaredTerms = GetTermsFromModel();
            foreach (var term in queryTerms)
            {
                var declaredTerm = GetDeclaredTerm(declaredTerms, term.Name);

                if (declaredTerm == null)
                    continue;

                yield return new FilterTerm
                {
                    InvalidSyntax = term.InvalidSyntax,
                    InvalidTerm = term.InvalidTerm,
                    InvalidOperator = term.InvalidOperator,
                    InvalidValue = term.InvalidValue,
                    Name = declaredTerm.Name,
                    EntityName = declaredTerm.EntityName,
                    Operator = term.Operator,
                    Value = term.Value,
                    ExpressionProvider = declaredTerm.ExpressionProvider
                };
            }
        }

        private static FilterTerm GetDeclaredTerm(IEnumerable<FilterTerm> declaredTerms, string name)
        {
            return declaredTerms.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var terms = GetValidTerms().ToArray();
            if (!terms.Any()) return query;

            var modifiedQuery = query;

            foreach (var term in terms)
            {
                var propertyInfo = ExpressionHelper.GetPropertyInfo<TEntity>(term.EntityName ?? term.Name);
                var obj = ExpressionHelper.Parameter<TEntity>();

                // Build up the LINQ expression backwards:
                // query = query.Where(x => x.Property == "Value");

                // x.Property
                var left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);

                // "Value"
                var right = term.ExpressionProvider.GetValue(term.Value);

                // x.Property == "Value"
                var comparisonExpression = term.ExpressionProvider.GetComparison(left, term.Operator, right);

                // x => x.Property == "Value"
                var lambdaExpression = ExpressionHelper.GetLambda<TEntity, bool>(obj, comparisonExpression);

                // query = query.Where...
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }

            return modifiedQuery;
        }

        private static IEnumerable<FilterTerm> GetTermsFromModel()
            => typeof(T).GetTypeInfo()
                .DeclaredProperties
                .Where(p => p.GetCustomAttributes<FiltrableAttribute>().Any())
                .Select(p =>
                {
                    var attribute = p.GetCustomAttribute<FiltrableAttribute>();
                    return new FilterTerm
                    {
                        Name = p.Name,
                        EntityName = attribute.EntityProperty,
                        ExpressionProvider = attribute.ExpressionProvider
                    };
                });
    }
}