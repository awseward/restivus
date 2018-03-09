using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Restivus
{
    // TODO: Find a better name
    public interface ISingleMethodRequestSender
    {
        HttpMethod HttpMethod { get; }

        Task<string> SendAsync(string path);

        Task<string> SendAsync(string path, CancellationToken cancellationToken);

        Task<string> SendAsync(
            string path,
            Action<HttpRequestMessage> mutateRequest);

        Task<string> SendAsync(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            CancellationToken cancellationToken);

        Task<string> SendAsync<TPayload>(
            string path,
            TPayload payload);

        Task<string> SendAsync<TPayload>(
            string path,
            TPayload payload,
            CancellationToken cancellationToken);

        Task<string> SendAsync<TPayload>(
            string path,
            Func<TPayload> getPayload);

        Task<string> SendAsync<TPayload>(
            string path,
            Func<TPayload> getPayload,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TResponse>(
            string path,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            TPayload payload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync);

        Task<TResponse> SendAsync<TPayload, TResponse>(
            string path,
            Func<TPayload> getPayload,
            Action<HttpRequestMessage> mutateRequest,
            Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync,
            CancellationToken cancellationToken);
    }

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

        public Task<string> SendAsync(string path, CancellationToken cancellationToken) =>
            SendAsync(
                path,
                request => { },
                cancellationToken
            );

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest)
        {
            return SendAsync(path, mutateRequest, CancellationToken.None);
        }

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest, CancellationToken cancellationToken)
        {
            return SendAsync(
                path,
                mutateRequest,
                async response => await response.Content.ReadAsStringAsync(),
                cancellationToken
            );
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload)
        {
            return SendAsync(path, payload, CancellationToken.None);
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload, CancellationToken cancellationToken)
        {
            return SendAsync(
                path,
                () => payload,
                cancellationToken
            );
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload)
        {
            return SendAsync(path, getPayload, CancellationToken.None);
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload, CancellationToken cancellationToken)
        {
            return SendAsync(
                path,
                getPayload,
                async response => await response.Content.ReadAsStringAsync(),
                cancellationToken
            );
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(path, deserializeResponseAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return SendAsync(path, request => { }, deserializeResponseAsync, cancellationToken);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(path, mutateRequest, deserializeResponseAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return _RestClient.SendAsync(HttpMethod, path, mutateRequest, deserializeResponseAsync, cancellationToken);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(path, payload, deserializeResponseAsync, CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return SendAsync(path, () => payload, deserializeResponseAsync, cancellationToken);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(
                path,
                getPayload,
                deserializeResponseAsync,
                CancellationToken.None);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return SendAsync(
                path,
                getPayload,
                _ => { },
                deserializeResponseAsync,
                cancellationToken
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(
                path,
                payload,
                mutateRequest,
                deserializeResponseAsync,
                CancellationToken.None
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return SendAsync(
                path,
                () => payload,
                mutateRequest,
                deserializeResponseAsync,
                cancellationToken
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync)
        {
            return SendAsync(
                path,
                getPayload,
                mutateRequest,
                deserializeResponseAsync,
                CancellationToken.None
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeResponseAsync, CancellationToken cancellationToken)
        {
            return _RestClient.SendAsync(
                HttpMethod,
                path,
                request =>
                {
                    request.Content = _createRequestContent(getPayload());
                    mutateRequest(request);
                },
                deserializeResponseAsync,
                cancellationToken
            );
        }
    }
}
