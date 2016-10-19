using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

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
                    if (queryParams.ContainsKey(key))
                    {
                        queryParams[key] = "__FILTERED__";
                    }
                }

                return queryParams;
            });
        }

        public static HttpRequestMessage FilterQueryParams(this HttpRequestMessage request, params string[] keys)
        {
            return request.UpdateRequestUri(uri => uri.FilterQueryParams(keys));
        }

        public static Uri SetQueryParams(this Uri uri, string key, string value)
        {
            return uri.UpdateQueryParams(queryParams =>
            {
                queryParams[key] = value;
                return queryParams;
            });
        }

        public static HttpRequestMessage SetQueryParams(this HttpRequestMessage request, string key, string value)
        {
            return request.UpdateRequestUri(uri => uri.SetQueryParams(key, value));
        }

        public static Uri SetQueryParams(this Uri uri, IDictionary<string, string> queryParams)
        {
            return uri.UpdateQueryParams(qParams =>
            {
                foreach (var kvp in queryParams)
                {
                    qParams[kvp.Key] = kvp.Value;
                }

                return qParams;
            });
        }

        public static HttpRequestMessage SetQueryParams(this HttpRequestMessage request, IDictionary<string, string> queryParams)
        {
            return request.UpdateRequestUri(uri => uri.SetQueryParams(queryParams));
        }

        public static Uri UpdateQueryParams(this Uri uri, Func<IDictionary<string, StringValues>, IDictionary<string, StringValues>> updateFn)
        {
            return new UriBuilder(uri)
            {
                Query = updateFn(QueryHelpers.ParseQuery(uri.Query)).ToString(),
            }.Uri;
        }

        public static HttpRequestMessage UpdateQueryParams(this HttpRequestMessage request, Func<IDictionary<string, StringValues>, IDictionary<string, StringValues>> updateFn)
        {
            return request.UpdateRequestUri(uri => uri.UpdateQueryParams(updateFn));
        }

        public static HttpRequestMessage UpdateRequestUri(this HttpRequestMessage request, Func<Uri, Uri> updateFn)
        {
            request.RequestUri = updateFn(request.RequestUri);

            return request;
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
