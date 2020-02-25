using System;

namespace ToqueToqueApi.Infrastructures
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FiltrableDoubleAttribute : FiltrableAttribute
    {
        public FiltrableDoubleAttribute()
        {
            ExpressionProvider = new DoubleFilterExpressionProvider();
        }
    }
}