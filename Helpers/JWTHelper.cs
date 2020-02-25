using System.Linq;
using System.Security.Claims;

namespace ToqueToqueApi.Helpers
{
    public static class JWTHelper
    {
        public static int GetUserId(ClaimsIdentity identity)
        {
            var result = int.Parse(identity.Claims.First(x => x.Type == ClaimTypes.Name).Value);

            return result;
        }
    }
}