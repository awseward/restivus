﻿using System;
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

        [Obsolete("Don't worry about implementing this yet, since it's brand new")]
        Task<string> SendAsync<TPayload>(
            string path,
            Func<TPayload> getPayload);

        [Obsolete("Don't worry about implementing this yet, since it's brand new")]
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

        public Task<string> SendAsync(string path) => SendAsync(path, message => { });

        public Task<string> SendAsync(string path, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest)
        {
            return SendAsync(
                path,
                mutateRequest,
                async response => await response.Content.ReadAsStringAsync());
        }

        public Task<string> SendAsync(string path, Action<HttpRequestMessage> mutateRequest, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload)
        {
            return SendAsync(
                path,
                payload,
                async response => await response.Content.ReadAsStringAsync());
        }

        public Task<string> SendAsync<TPayload>(string path, TPayload payload, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload)
        {
            throw new NotImplementedException();
        }

        public Task<string> SendAsync<TPayload>(string path, Func<TPayload> getPayload, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(path, request => { }, deserializeAsync);
        }


        public Task<TResponse> SendAsync<TResponse>(string path, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return _RestClient.SendAsync(HttpMethod, path, mutateRequest, deserializeAsync);
        }

        public Task<TResponse> SendAsync<TResponse>(string path, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(path, () => payload, deserializeAsync);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(
                path,
                getPayload,
                _ => { },
                deserializeAsync);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload payload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return SendAsync(
                path,
                () => payload,
                mutateRequest,
                deserializeAsync);
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, TPayload getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync)
        {
            return _RestClient.SendAsync(
                HttpMethod,
                path,
                request =>
                {
                    request.Content = _createRequestContent(getPayload());
                    mutateRequest(request);
                },
                deserializeAsync
            );
        }

        public Task<TResponse> SendAsync<TPayload, TResponse>(string path, Func<TPayload> getPayload, Action<HttpRequestMessage> mutateRequest, Func<HttpResponseMessage, Task<TResponse>> deserializeAsync, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
