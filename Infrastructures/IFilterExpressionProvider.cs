using System.Collections.Generic;
using System.Linq.Expressions;

namespace ToqueToqueApi.Infrastructures
{
    public interface IFilterExpressionProvider
    {
        IEnumerable<string> GetOperators();

        ConstantExpression GetValue(string input);

        Expression GetComparison(
            MemberExpression left,
            string op,
            ConstantExpression right);

        bool IsValidValue(string input);
    }
}