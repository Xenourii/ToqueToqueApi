using System;
using System.Linq.Expressions;

namespace ToqueToqueApi.Infrastructures
{
    public class IntegerFilterExpressionProvider : ComparableFilterExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!int.TryParse(input, out var value))
                throw new ArgumentException("Invalid search value.");

            return Expression.Constant(value);
        }

        public override bool IsValidValue(string input) => int.TryParse(input, out _);
    }
}