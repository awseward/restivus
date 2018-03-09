using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// This overload's use is discouraged, as it can cause subtle misbehavior when
        /// attempting to await a Task and only catch an Exception it may throw.
        /// </summary>
        /// <example>
        /// try
        /// {
        ///     await restClient
        ///         .SendAsync(
        ///             HttpMethod.Get,
        ///             "/health_check",
        ///             request => { },
        ///             async response =>
        ///             {
        ///                 if (_DetectFailureState(response))
        ///                 {
        ///                     var ex = await _ConvertFailedResponseToRelevantException(response);
        ///                     throw ex;
        ///                 }
        ///
        ///                 // The correctly intended overload will be chosen if you uncomment the
        ///                 // following line, but callsites are bound to be missing this detail
        ///                 //
        ///                 // return Task.CompletedTask;
        ///             });
        /// }
        /// catch (Exception ex)
        /// {
        ///     // Execution will not reach this spot.
        /// }
        ///
        /// </example>
        [Obsolete("Prefer deserializeResponseAsync overload")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, TResponse> deserializeResponse)
        {
            return client.SendAsync(
                method,
                relativePath,
                mutateRequest,
                deserializeResponse,
                CancellationToken.None
            );
        }

        /// <summary>
        /// This overload's use is discouraged, as it can cause subtle misbehavior when
        /// attempting to await a Task and only catch an Exception it may throw.
        /// </summary>
        /// <example>
        /// try
        /// {
        ///     await restClient
        ///         .SendAsync(
        ///             HttpMethod.Get,
        ///             "/health_check",
        ///             request => { },
        ///             async response =>
        ///             {
        ///                 if (_DetectFailureState(response))
        ///                 {
        ///                     var ex = await _ConvertFailedResponseToRelevantException(response);
        ///                     throw ex;
        ///                 }
        ///
        ///                 // The correctly intended overload will be chosen if you uncomment the
        ///                 // following line, but callsites are bound to be missing this detail
        ///                 //
        ///                 // return Task.CompletedTask;
        ///             },
        ///             someCancellationToken);
        /// }
        /// catch (Exception ex)
        /// {
        ///     // Execution will not reach this spot.
        /// }
        ///
        /// </example>
        [Obsolete("Prefer deserializeResponseAsync overload")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, TResponse> deserializeResponse,
            CancellationToken cancellationToken)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequest(message);

            return client.RequestSender.SendAsync(
                message,
                deserializeResponse,
                cancellationToken
            );
        }

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return client.SendAsync(
                method,
                relativePath,
                mutateRequest,
                deserializeResponseAsync,
                CancellationToken.None
            );
        }

        public static Task<TResponse> SendAsync<TResponse>(
            this IRestClient client,
            HttpMethod method,
            string relativePath,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken)
        {
            var message = client.CreateRequestMessage(method, relativePath);

            mutateRequest(message);

            return client.RequestSender.SendAsync(
                message,
                deserializeResponseAsync,
                cancellationToken
            );
        }
    }
}
