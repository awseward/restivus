using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Restivus
{
    public static class Extensions
    {
        public static StringContent AsJsonContent(this string json)
        {
            var safeJson = string.IsNullOrWhiteSpace(json)
                ? string.Empty
                : json;

            return new StringContent(safeJson, Encoding.UTF8, "application/json");
        }

        public static Uri FilterQueryParams(this Uri uri, params string[] keys)
        {
            var paramCollection = HttpUtility.ParseQueryString(uri.Query);

            foreach (var key in keys)
            {
                if (paramCollection.AllKeys.Contains(key))
                {
                    paramCollection[key] = "__FILTERED__";
                }
            }

            return new UriBuilder(uri)
            {
                Query = paramCollection.ToString(),
            }.Uri;
        }

        public static Func<T, T> Identity<T>() => x => x;

        public static Func<T, T> AsNoOpIfNull<T>(this Func<T, T> function)
        {
            return function ?? Identity<T>();
        }

        public static Func<T, T> AsFluent<T>(this Action<T> action)
        {
            return (action == null)
                ? Identity<T>()
                : x => { action(x); return x; };
        }
    }
}
