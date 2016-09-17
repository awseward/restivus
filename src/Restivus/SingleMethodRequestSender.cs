using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restivus
{
    public interface IChainableRequestSender
    {
        HttpMethod HttpMethod { get; }

        Task<string> SendAsync(string path);

        Task<string> SendAsync(string path, CancellationToken token);

        Task<string> SendAsync(
            string path,
            Action<HttpRequestMessage> mutateRequest);

        Task<string> SendAsync(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            CancellationToken token);

        Task<string> SendAsync<TPayload>(
            string path,
            TPayload payload);

        Task<string> SendAsync<TPayload>(
            string path,
            TPayload payload,
            CancellationToken token);

        Task<string> SendAsync<TPayload>(
            string path,
            Func<TPayload> getPayload);

        Task<string> SendAsync<TPayload>(
            string path,
            Func<TPayload> getPayload,
            CancellationToken token);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeAsync,
            CancellationToken token);
    }

    [Obsolete("Prefer IChainableRequestSender")]
    public interface ISingleMethodRequestSender : IChainableRequestSender { }

    public class SingleMethodRequestSender : ISingleMethodRequestSender
    {
        public SingleMethodRequestSender(
            IRestClient restClient,
            HttpMethod httpMethod,
            Func<object, HttpContent> createRequestContent) // Not too thrilled with this last parameter...
        {
            HttpMethod = httpMethod;
            _RestClient = restClient;
            _createRequestContent = createRequestContent;
        }

        readonly IRestClient _RestClient;

        readonly Func<object, HttpContent> _createRequestContent;

        public HttpMethod HttpMethod { get; }

        public Task<string> SendAsync(string path) => SendAsync(path, CancellationToken.None);

        public Task<string> SendAsync(string path, CancellationToken token) =>
            SendAsync(
                path,
                request => { },
                token
            );

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest)
        {
            return SendAsync(path, mutateRequest, CancellationToken.None);
        }

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest, CancellationToken token)
        {
            return SendAsync(
                path,
                mutateRequest,
                async response => await response.Content.ReadAsStringAsync(),
                token
            );
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload)
        {
            return SendAsync(path, payload, CancellationToken.None);
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload, CancellationToken token)
        {
            return SendAsync(
                path,
                () => payload,
                token
            );
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload)
        {
            return SendAsync(path, getPayload, CancellationToken.None);
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload, CancellationToken token)
        {
            return SendAsync(
                path,
                getPayload,
                async response => await response.Content.ReadAsStringAsync(),
                token
            );
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(path, deserializeAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return SendAsync(path, request => { }, deserializeAsync, token);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(path, mutateRequest, deserializeAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return _RestClient.SendAsync(HttpMethod, path, mutateRequest, deserializeAsync, token);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(path, payload, deserializeAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return SendAsync(path, () => payload, deserializeAsync, token);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(
                path,
                getPayload,
                deserializeAsync,
                CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return SendAsync(
                path,
                getPayload,
                _ => { },
                deserializeAsync,
                token
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(
                path,
                payload,
                mutateRequest,
                deserializeAsync,
                CancellationToken.None
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return SendAsync(
                path,
                () => payload,
                mutateRequest,
                deserializeAsync,
                token
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(
                path,
                getPayload,
                mutateRequest,
                deserializeAsync,
                CancellationToken.None
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            return _RestClient.SendAsync(
                HttpMethod,
                path,
                request =>
                {
                    request.Content = _createRequestContent(getPayload());
                    mutateRequest(request);
                },
                deserializeAsync,
                token
            );
        }
    }
}
