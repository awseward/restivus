using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Restivus
{
    public interface IHttpRequestSender
    {
        HttpClient HttpClient { get; }
        Task SendAsync(HttpRequestMessage message);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContent);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<Task<string>, T> deserializeResponseContent);
        Task<T> SendAsync<T>(HttpRequestMessage message, Func<string, T> deserializeResponseContent);
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

        public Task SendAsync(HttpRequestMessage message) => SendAsync<object>(message, _ => null);

        public async Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, Task<T>> deserializeResponseContentAsync)
        {
            Logger?.Debug("{message}", message);

            using (message)
            using (var response = await HttpClient.SendAsync(message))
            {
                response.EnsureSuccessStatusCode();

                Logger?.Debug("{@response}", response);

                return await deserializeResponseContentAsync(response);
            }
        }

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<HttpResponseMessage, T> deserializeResponseContent)
        {
            return SendAsync(
                message,
                response => Task.FromResult(deserializeResponseContent(response))
            );
        }

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<Task<string>, T> deserializeResponseContent)
        {
            return SendAsync(
                message,
                response => deserializeResponseContent(response.Content.ReadAsStringAsync())
            );
        }

        public Task<T> SendAsync<T>(HttpRequestMessage message, Func<string, T> deserializeResponseContent)
        {
            return SendAsync(
                message,
                async response =>
                {
                    return deserializeResponseContent(await response.Content.ReadAsStringAsync());
                }
            );
        }
    }
}
