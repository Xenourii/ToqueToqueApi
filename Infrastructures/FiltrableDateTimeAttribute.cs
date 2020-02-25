using System;

namespace ToqueToqueApi.Infrastructures
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FiltrableDateTimeAttribute : FiltrableAttribute
    {
        public FiltrableDateTimeAttribute()
        {
            ExpressionProvider = new DateTimeFilterExpressionProvider();
        }
    }
}