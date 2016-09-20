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
    public interface IHttpRequestSender
    {
        HttpClient HttpClient { get; }
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken token);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContentAsync);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContentAsync, CancellationToken token);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent, CancellationToken token);
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

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message) => SendAsync(message, Task.FromResult);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken token) =>
            SendAsync(
                message,
                Task.FromResult,
                CancellationToken.None
            );

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContentAsync) =>
            SendAsync(
                message,
                deserializeResponseContentAsync,
                CancellationToken.None
            );

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent) =>
            SendAsync(
                message,
                deserializeResponseContent,
                CancellationToken.None
            );

        public async Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContentAsync, CancellationToken token)
        {
            Logger?.Debug("{@message}", message);

            using (message)
            using (var response = await HttpClient.SendAsync(message, token))
            {
                Logger?.Debug("{@response}", response);

                var responseContent = await deserializeResponseContentAsync(response);

                Logger?.Debug("{@responseContent}", responseContent);

                return responseContent;
            }
        }

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent, CancellationToken token) =>
            SendAsync(
                message,
                response => Task.FromResult(deserializeResponseContent(response)),
                token
            );
    }
}
