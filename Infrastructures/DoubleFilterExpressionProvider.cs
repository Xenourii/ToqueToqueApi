using System;
using System.Linq.Expressions;

namespace ToqueToqueApi.Infrastructures
{
    public class DoubleFilterExpressionProvider : ComparableFilterExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            if (!double.TryParse(input, out var value))
                throw new ArgumentException("Invalid search value.");

            return Expression.Constant(value);
        }

        public override bool IsValidValue(string input) => double.TryParse(input, out _);
    }
}