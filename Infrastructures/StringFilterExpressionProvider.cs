using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ToqueToqueApi.Helpers;

namespace ToqueToqueApi.Infrastructures
{
    public class StringFilterExpressionProvider : DefaultFilterExpressionProvider
    {
        private const string StartsWithOperator = "sw";
        private const string EndsWithOperator = "ew";
        private const string ContainsOperator = "co";

        private readonly MethodInfo _iLikeMethod = ExtractMethod(()
            => EF.Functions.ILike(string.Empty, string.Empty));

        private static MethodInfo ExtractMethod(Expression<Action> expr) => ((MethodCallExpression) expr.Body).Method;

        private static readonly MethodInfo StartsWithMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == ConstantsHelper.StartsWithFunctionName && m.GetParameters().Length == 1);

        private static readonly MethodInfo EndsWithMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == ConstantsHelper.EndsWithFunctionName && m.GetParameters().Length == 1);

        private static readonly MethodInfo StringEqualsMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == ConstantsHelper.EqualsFunctionName && m.GetParameters().Length == 1);

        private static readonly MethodInfo ContainsMethod = typeof(string)
            .GetMethods()
            .First(m => m.Name == ConstantsHelper.ContainsFunctionName && m.GetParameters().Length == 1);

        //When issue will be resolved with Entity Framework Core, use like  return Expression.Call(left, StartsWithMethod, right, IgnoreCase);
        private static readonly ConstantExpression IgnoreCase = Expression.Constant(StringComparison.OrdinalIgnoreCase);

        public override IEnumerable<string> GetOperators()
            => base.GetOperators()
                .Concat(new[]
                {
                    StartsWithOperator,
                    EndsWithOperator,
                    ContainsOperator
                });

        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            switch (op.ToLower())
            { 
                case StartsWithOperator:
                    return GetILikeExpression(left, $"{right.Value}%");
                case EndsWithOperator:
                    return GetILikeExpression(left, $"%{right.Value}");
                case ContainsOperator:
                    return GetILikeExpression(left, $"%{right.Value}%");
                case EqualsOperator:
                    return GetILikeExpression(left, $"{right.Value}");

                default: return base.GetComparison(left, op, right);
            }
        }

        private Expression GetILikeExpression(Expression left, string matchExpression) =>
            Expression.Call(
                _iLikeMethod,
                Expression.Constant(EF.Functions),
                left,
                Expression.Constant(matchExpression, typeof(string)));
    }
}