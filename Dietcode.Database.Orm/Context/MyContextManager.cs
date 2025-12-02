using Dietcode.Database.Domain;
using Microsoft.AspNetCore.Http;

namespace Dietcode.Database.Orm.Context
{
    public class MyContextManager<T> : IMyContextManager<T> where T : class, new()
    {
        private const string CONTEXT_KEY = "ContextManager.Context";
        private readonly IHttpContextAccessor context;

        public MyContextManager(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public T GetContext()
        {
            var items = context.HttpContext.Items;

            if (items[CONTEXT_KEY] is T ctx)
                return ctx;

            var novo = new T();
            items[CONTEXT_KEY] = novo;
            return novo;
        }
    }
}
