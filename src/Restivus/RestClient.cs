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

        public static Task<TResponse> SendAsync<TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, TResponse> deserializeResponse)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message, deserializeResponse);
        }

        public static Task<TResponse> SendAsync<TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(message, deserializeResponseAsync);
        }

        static Action<HttpRequestMessage> _JsonPayloadSetter<TPayload>(Func<TPayload> getPayload) =>
            message => message.Content = JsonConvert.SerializeObject(getPayload()).AsJsonContent();

        public static Task<TResponse> SendJsonAsync<TPayload, TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, TResponse> deserializeResponse)
        {
            return client.SendAsync(
                method,
                relativePath,
                _JsonPayloadSetter(getPayload),
                deserializeResponse
            );
        }

        public static Task<TResponse> SendJsonAsync<TPayload, TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return client.SendAsync(
                method,
                relativePath,
                _JsonPayloadSetter(getPayload),
                deserializeResponseAsync
            );
        }

        public static Task<TResponse> SendJsonAsync<TPayload, TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            TPayload payload,
            Func<HttpResponseMessage, TResponse> deserializeResponse)
        {
            return client.SendJsonAsync(
                method,
                relativePath,
                () => payload,
                deserializeResponse
            );
        }

        public static Task<TResponse> SendJsonAsync<TPayload, TResponse>(this IRestClient client,
            HttpMethod method,
            string relativePath,
            TPayload payload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
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
