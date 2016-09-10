using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Restivus
{
    public interface IRestClient
    {
        IWebApi WebApi { get; }
        IReadOnlyCollection<IHttpRequestMiddleware> RequestMiddlewares { get; }
        IHttpRequestSender RequestSender { get; }
    }

    public static class RestClientExtensions
    {
        public static HttpRequestMessage CreateRequestMessage(this IRestClient client,
            HttpMethod method,
            string path)
        {
            var message = new HttpRequestMessage(
                method,
                client.WebApi.UriForAbsolutePath(path)
            );

            return client.RequestMiddlewares.Aggregate(
                message,
                (msg, middleware) => middleware?.Run(message)
            );
        }

        public static Task<T> SendAsync<T>(this IRestClient client,
            HttpMethod method,
            string absolutePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, T> deserializeResponse)
        {
            var message = client.CreateRequestMessage(method, absolutePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message, deserializeResponse);
        }

        public static Task SendAsync(this IRestClient client,
            HttpMethod method,
            string absolutePath,
            Action<HttpRequestMessage> mutateRequestMessage)
        {
            var message = client.CreateRequestMessage(method, absolutePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message);
        }
    }
}
