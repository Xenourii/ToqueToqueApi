using System;

namespace ToqueToqueApi.Infrastructures
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FiltrableStringAttribute : FiltrableAttribute
    {
        public FiltrableStringAttribute()
        {
            ExpressionProvider = new StringFilterExpressionProvider();
        }
    }
}