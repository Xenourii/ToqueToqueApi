using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ToqueToqueApi.Infrastructures
{
    public class DecimalFilterExpressionProvider : ComparableFilterExpressionProvider
    {
        public override ConstantExpression GetValue(string input)
        {
            const NumberStyles style = NumberStyles.AllowDecimalPoint;
            var culture = CultureInfo.InvariantCulture;

            if (!decimal.TryParse(input, style, culture, out var value))
                throw new ArgumentException("Invalid search value.");

            return Expression.Constant(value);
        }

        public override bool IsValidValue(string input) => decimal.TryParse(input, out _);
    }
}