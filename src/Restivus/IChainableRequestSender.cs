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
}
