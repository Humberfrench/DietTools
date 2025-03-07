using Dietcode.Database.Net.Domain;
using System;
using System.Web;

namespace Dietcode.Database.Net.Orm.Context
{
    public class ContextManager<T> : IContextManager<T> where T : class, new()
    {
        private const string CONTEXT_KEY = "ContextManager.Context";

        public T GetContext()
        {
            if (HttpContext.Current == null)
                return new T();

            if (HttpContext.Current.Items[CONTEXT_KEY] == null)
            {
                HttpContext.Current.Items[CONTEXT_KEY] = new T();
            }

            return HttpContext.Current.Items[CONTEXT_KEY] as T;
        }
    }
}
