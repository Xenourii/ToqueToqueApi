using System;
using System.Collections.Generic;

namespace ToqueToqueApi.Infrastructures
{
    public class FilterTerm
    {
        public string Name { get; set; }
        public string EntityName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool InvalidSyntax { get; set; }
        public bool InvalidTerm { get; set; }
        public bool InvalidOperator { get; set; }
        public bool InvalidValue { get; set; }
        public IFilterExpressionProvider ExpressionProvider { get; set; }
    }
}