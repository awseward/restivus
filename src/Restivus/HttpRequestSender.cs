using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restivus
{
    internal static class Deprecations
    {
        internal const string NO_CANCELLATION = "Prefer alternative accepting a System.Threading.CancellationToken";
    }

    public interface IHttpRequestSender
    {
        HttpClient HttpClient { get; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, Task<T>> deserializeResponseAsync, CancellationToken cancellationToken);
        Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, T> deserializeResponse, CancellationToken cancellationToken);

        [Obsolete(message: Deprecations.NO_CANCELLATION)]
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        [Obsolete(message: Deprecations.NO_CANCELLATION)]
        Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, T> deserializeResponse);
        [Obsolete(message: Deprecations.NO_CANCELLATION)]
        Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, Task<T>> deserializeResponseAsync);
    }

    public partial class HttpRequestSender
    {
        public static HttpClient DefaultHttpClient { get; } = new HttpClient();

        public static IHttpRequestSender Default { get; } =
            new HttpRequestSender(DefaultHttpClient);
    }

    public partial class HttpRequestSender : IHttpRequestSender
    {
        public HttpRequestSender(HttpClient client, ILogger logger)
        {
            HttpClient = client;
            Logger = logger;
        }

        public HttpRequestSender(HttpClient client) : this(client, null) { }

        public HttpClient HttpClient { get; }

        public ILogger Logger { get; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => SendAsync(request, Task.FromResult);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            SendAsync(
                request,
                Task.FromResult,
                CancellationToken.None
            );

        public Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, Task<T>> deserializeResponseAsync) =>
            SendAsync(
                request,
                deserializeResponseAsync,
                CancellationToken.None
            );

        public Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, T> deserializeResponse) =>
            SendAsync(
                request,
                deserializeResponse,
                CancellationToken.None
            );

        public async Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, Task<T>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            Logger?.Debug("{@message}", request);

            using (request)
            using (var response = await HttpClient.SendAsync(request, cancellationToken))
            {
                Logger?.Debug("{@response}", response);

                var responseContent = await deserializeResponseAsync(response);

                {
                    var asResponseMessage = responseContent as HttpResponseMessage;

                    if (asResponseMessage == null || asResponseMessage != response)
                    {
                        Logger?.Debug("{@responseContent}", responseContent);
                    }
                }

                return responseContent;
            }
        }

        public Task<T> SendAsync<T>(HttpRequestMessage request, Func<HttpResponseMessage, T> deserializeResponse, CancellationToken cancellationToken) =>
            SendAsync(
                request,
                response => Task.FromResult(deserializeResponse(response)),
                cancellationToken
            );
    }
}
