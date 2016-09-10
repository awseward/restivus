using Newtonsoft.Json;
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
        static HttpRequestMessage _CreateRequestMessage(this IRestClient client,
            HttpMethod method,
            string path,
            Func<string, Uri> buildUriFromPath)
        {
            var message = new HttpRequestMessage(
                method,
                buildUriFromPath(path)
            );

            return client.RequestMiddlewares.Aggregate(
                message,
                (msg, middleware) => middleware?.Run(message)
            );
        }

        public static HttpRequestMessage CreateRequestMessageForRelativePath(this IRestClient client,
            HttpMethod method,
            string path)
        {
            return client._CreateRequestMessage(
                method,
                path,
                client.WebApi.UriForRelativePath
            );
        }

        public static HttpRequestMessage CreateRequestMessageForAbsolutePath(this IRestClient client,
            HttpMethod method,
            string path)
        {
            return client._CreateRequestMessage(
                method,
                path,
                client.WebApi.UriForAbsolutePath
            );
        }

        public static HttpRequestMessage CreateRequestMessage(this IRestClient client,
            HttpMethod method,
            string path) => client.CreateRequestMessageForRelativePath(method, path);

        public static Task<T> SendAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, T> deserializeResponse)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message, deserializeResponse);
        }

        public static Task<T> SendAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, Task<T>> deserializeResponseAsync)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message, deserializeResponseAsync);
        }

        public static Task<T> SendJsonAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Func<T> getPayload,
            Func<HttpResponseMessage, T> deserializeResponse)
        {
            return client.SendAsync(
                method,
                relativePath,
                message => message.Content = JsonConvert.SerializeObject(getPayload()).AsJsonContent(),
                deserializeResponse
            );
        }

        public static Task<T> SendJsonAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Func<T> getPayload,
            Func<HttpResponseMessage, Task<T>> deserializeResponseAsync)
        {
            return client.SendAsync(
                method,
                relativePath,
                message => message.Content = JsonConvert.SerializeObject(getPayload()).AsJsonContent(),
                deserializeResponseAsync
            );
        }

        public static Task<T> SendJsonAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            T payload,
            Func<HttpResponseMessage, T> deserializeResponse)
        {
            return client.SendJsonAsync(
                method,
                relativePath,
                () => payload,
                deserializeResponse
            );
        }

        public static Task<T> SendJsonAsync<T>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            T payload,
            Func<HttpResponseMessage, Task<T>> deserializeResponseAsync)
        {
            return client.SendJsonAsync(
                method,
                relativePath,
                () => payload,
                deserializeResponseAsync
            );
        }
    }
}
