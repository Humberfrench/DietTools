using Dietcode.Database.Domain;

namespace Dietcode.Database.Orm.Context
{
    public class MyContextManager<T> : IMyContextManager<T> where T : class, new()
    {
        private const string CONTEXT_KEY = "ContextManager.Context";
        private readonly IAmbientContextStore store;

        public MyContextManager(IAmbientContextStore store)
        {
            this.store = store;
        }

        public T GetContext()
        {
            if (store.TryGet<T>(CONTEXT_KEY, out var ctx))
                return ctx;

            var novo = new T();
            store.Set(CONTEXT_KEY, novo);
            return novo;
        }
    }


}

