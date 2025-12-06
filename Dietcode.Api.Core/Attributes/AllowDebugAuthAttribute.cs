using Microsoft.AspNetCore.Authorization;

namespace Dietcode.Api.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AllowDebugAuthAttribute : AuthorizeAttribute
    {
        public AllowDebugAuthAttribute()
        {
            Policy = "AllowDebug";
        }
    }
}
