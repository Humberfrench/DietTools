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
            var http = context.HttpContext;

            // Jobs/serviços: não existe HttpContext
            if (http is null)
                return new T();

            // Web: 1 contexto por request via Items
            var items = http.Items;

            if (items.TryGetValue(CONTEXT_KEY, out var existing) && existing is T ctx)
                return ctx;

            var novo = new T();
            items[CONTEXT_KEY] = novo;
            return novo;
        }
    }
}
