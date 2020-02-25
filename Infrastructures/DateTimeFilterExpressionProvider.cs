using System;
using System.Linq.Expressions;

namespace ToqueToqueApi.Infrastructures
{
    public class DateTimeFilterExpressionProvider : ComparableFilterExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!DateTime.TryParse(input, out var value))
                throw new ArgumentException("Invalid search value.");

            return Expression.Constant(value);
        }

        public override bool IsValidValue(string input) => DateTime.TryParse(input, out _);
    }
}