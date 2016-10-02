using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            return uri.UpdateQueryParams(queryParams =>
            {
                foreach (var key in keys)
                {
                    if (queryParams.AllKeys.Contains(key))
                    {
                        queryParams[key] = "__FILTERED__";
                    }
                }

                return queryParams;
            });
        }

        public static Uri SetQueryParams(this Uri uri, string key, string value)
        {
            return uri.UpdateQueryParams(queryParams =>
            {
                queryParams.Set(key, value);
                return queryParams;
            });
        }

        public static Uri SetQueryParams(this Uri uri, IDictionary<string, string> queryParams)
        {
            return uri.UpdateQueryParams(qParams =>
            {
                foreach (var kvp in queryParams)
                {
                    qParams.Set(kvp.Key, kvp.Value);
                }

                return qParams;
            });
        }

        public static Uri UpdateQueryParams(this Uri uri, Func<NameValueCollection, NameValueCollection> updateFn)
        {
            return new UriBuilder(uri)
            {
                Query = updateFn(HttpUtility.ParseQueryString(uri.Query)).ToString(),
            }.Uri;
        }

        public static Func<T, T> Identity<T>() => x => x;

        public static Func<T, T> AsIdentityIfNull<T>(this Func<T, T> function)
        {
            return function ?? Identity<T>();
        }

        public static Func<T, T> AsFluent<T>(this Action<T> action)
        {
            return (action == null)
                ? Identity<T>()
                : x => { action(x); return x; };
        }

        [Obsolete("Prefere AsIdentityIfNull")]
        public static Func<T, T> AsNoOpIfNull<T>(this Func<T, T> function) => AsIdentityIfNull(function);
    }
}
