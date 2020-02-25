namespace ToqueToqueApi.Infrastructures
{
    public class FiltrableIntegerAttribute : FiltrableAttribute
    {
        public FiltrableIntegerAttribute()
        {
            ExpressionProvider = new IntegerFilterExpressionProvider();
        }
    }
}