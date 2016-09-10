using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restivus
{
    public interface IWebApi
    {
        string Scheme { get; }
        string Host { get; }
        int? Port { get; }
        string BasePath { get; }
    }

    public static class WebApiExtensions
    {
        public static Uri UriForAbsolutePath(this IWebApi webApi, string absolutePath)
        {
            return new UriBuilder
            {
                Scheme = webApi.Scheme,
                Host = webApi.Host,
                Port = webApi.Port ?? -1,
                Path = absolutePath,
            }.Uri;
        }

        public static Uri UriForRelativePath(this IWebApi webApi, string relativePath)
        {
            return new UriBuilder
            {
                Scheme = webApi.Scheme,
                Host = webApi.Host,
                Port = webApi.Port ?? -1,
                Path = _PathCombine(webApi.BasePath, relativePath),
            }.Uri;
        }

        static string _PathCombine(string prefix, string postfix)
        {
            return string.Join("/", new[]
            {
                prefix?.TrimEnd('/') ?? string.Empty,
                postfix?.TrimStart('/') ?? string.Empty,
            });
        }
    }
}
