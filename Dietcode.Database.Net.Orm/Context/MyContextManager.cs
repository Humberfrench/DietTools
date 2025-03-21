using Dietcode.Database.Net.Domain;
using System.Web;

namespace Dietcode.Database.Net.Orm.Context
{
    public class MyContextManager<T> : IMyContextManager<T> where T : class, new()
    {
        private const string ContextKey = "ContextManager.Context";

        public T GetContext()
        {
            if (HttpContext.Current == null)
                return new T();

            if (HttpContext.Current.Items[ContextKey] == null)
            {
                HttpContext.Current.Items[ContextKey] = new T();
            }

            return HttpContext.Current.Items[ContextKey] as T;
        }
    }
}
