using System;

namespace ToqueToqueApi.Infrastructures
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FiltrableDecimalAttribute : FiltrableAttribute
    {
        public FiltrableDecimalAttribute()
        {
            ExpressionProvider = new DecimalFilterExpressionProvider();
        }
    }
}