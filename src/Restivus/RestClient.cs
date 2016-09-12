using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        public static ISingleMethodRequestSender For(
            this IRestClient client,
            HttpMethod method,
            Func<object, HttpContent> buildContentFromPayload)
        {
            return new SingleMethodRequestSender(client, method, buildContentFromPayload);
        }

        public static ISingleMethodRequestSender Get(
            this IRestClient client) => client.For(HttpMethod.Get, _ => null);

        public static ISingleMethodRequestSender Put(
            this IRestClient client,
            Func<object, HttpContent> buildContent) => client.For(HttpMethod.Put, buildContent);

        public static ISingleMethodRequestSender Post(
            this IRestClient client,
            Func<object, HttpContent> buildContent) => client.For(HttpMethod.Post, buildContent);

        static HttpRequestMessage _CreateRequestMessage(
            this IRestClient client,
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

        public static HttpRequestMessage CreateRequestMessageForRelativePath(
            this IRestClient client,
            HttpMethod method,
            string path)
        {
            return client._CreateRequestMessage(
                method,
                path,
                client.WebApi.UriForRelativePath
            );
        }

        public static HttpRequestMessage CreateRequestMessageForAbsolutePath(
            this IRestClient client,
            HttpMethod method,
            string path)
        {
            return client._CreateRequestMessage(
                method,
                path,
                client.WebApi.UriForAbsolutePath
            );
        }

        public static HttpRequestMessage CreateRequestMessage(
            this IRestClient client,
            HttpMethod method,
            string path) => client.CreateRequestMessageForRelativePath(method, path);

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, TResponse> deserializeResponse)
        {
            return client.SendAsync(
                method,
                relativePath,
                mutateRequestMessage,
                deserializeResponse,
                CancellationToken.None
            );
        }

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, TResponse> deserializeResponse,
            CancellationToken token)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(
                message,
                deserializeResponse,
                token
            );
        }

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return client.SendAsync(
                method,
                relativePath,
                mutateRequestMessage,
                deserializeResponseAsync,
                CancellationToken.None
            );
        }

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequestMessage,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken token)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequestMessage(message);

            return client.RequestSender.SendAsync(
                message,
                deserializeResponseAsync,
                token
            );
        }
    }
}
