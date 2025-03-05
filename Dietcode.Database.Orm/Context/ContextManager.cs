using Dietcode.Database.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Dietcode.Database.Orm.Context
{
    public class ContextManager<T> : IContextManager<T> where T : class, new()
    {
        private const string CONTEXT_KEY = "ContextManager.Context";
        private readonly IHttpContextAccessor context;
        private readonly IConfiguration configuration;

        public ContextManager(IHttpContextAccessor context,
                              IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }


        public T GetContext()
        {
            if (context.HttpContext == null)
                return new T();

            if (context.HttpContext.Items[CONTEXT_KEY] == null)
            {
                context.HttpContext.Items[CONTEXT_KEY] = new T();
            }

            return (context.HttpContext.Items[CONTEXT_KEY] as T)!;
        }

    }
}
