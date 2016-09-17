using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restivus
{
    [Obsolete("Prefer IChainableRequestSender")]
    public interface ISingleMethodRequestSender : IChainableRequestSender
    {
        [Obsolete("Avoid using this, because it is being removed in 0.6.0")]
        HttpMethod HttpMethod { get; }
    }

    public class SingleMethodRequestSender : ISingleMethodRequestSender
    {
        public SingleMethodRequestSender(
            IRestClient restClient,
            HttpMethod httpMethod,
            Func<object, HttpContent> createRequestContent) // Not too thrilled with this last parameter...
        {
            _httpMethod = httpMethod;
            _RestClient = restClient;
            _createRequestContent = createRequestContent;
        }

        readonly IRestClient _RestClient;

        readonly Func<object, HttpContent> _createRequestContent;

        readonly HttpMethod _httpMethod;

        public HttpMethod HttpMethod => _httpMethod;

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
            return _RestClient.SendAsync(_httpMethod, path, mutateRequest, deserializeAsync, token);
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
                _httpMethod,
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
