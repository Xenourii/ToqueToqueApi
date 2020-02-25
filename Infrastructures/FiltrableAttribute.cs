using System;

namespace ToqueToqueApi.Infrastructures
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FiltrableAttribute : Attribute
    {
        public string EntityProperty { get; set; }

        public IFilterExpressionProvider ExpressionProvider { get; set; } = new DefaultFilterExpressionProvider();
    }
}