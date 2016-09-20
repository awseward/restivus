using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Restivus
{
    public interface IHttpRequestPipeline
    {
        HttpRequestMessage Run();
        HttpRequestMessage Run(HttpRequestMessage request);
    }

    public class HttpRequestPipeline : IHttpRequestPipeline
    {
        public HttpRequestPipeline(Func<HttpRequestMessage, HttpRequestMessage> run)
        {
            _run = run ?? (req => req);
        }

        readonly Func<HttpRequestMessage, HttpRequestMessage> _run;

        public HttpRequestMessage Run(HttpRequestMessage request) => _run(request);

        public HttpRequestMessage Run() => _run(new HttpRequestMessage());
    }

    public static class HttpRequestPipelineExtensions
    {
        public static Task<HttpResponseMessage> SendAsync(this IHttpRequestPipeline pipeline)
        {
            var request = pipeline.Run();

            throw new NotImplementedException();
        }

        public static Task<HttpResponseMessage> SendAsync<TRequestPayload>(this IHttpRequestPipeline pipeline, TRequestPayload requestPayload)
        {
            var request = pipeline.Run();

            throw new NotImplementedException();
        }

        public static async Task<TResponsePayload> SendAsync<TResponsePayload>(this IHttpRequestPipeline pipeline, Func<HttpResponseMessage, Task<TResponsePayload>> getResponsePayloadAsync)
        {
            var request = pipeline.Run();

            // TEMPORARY
            using (var client = new HttpClient())
            using (var response = await client.SendAsync(request))
            {
                return await getResponsePayloadAsync(response);
            }
        }

        public static Task<TResponsePayload> SendAsync<TRequestPayload, TResponsePayload>(this IHttpRequestPipeline pipeline, TRequestPayload payload, Func<HttpResponseMessage, Task<TResponsePayload>> getResponsePayloadAsync)
        {
            var request = pipeline.Run();

            throw new NotImplementedException();
        }

        public static IHttpRequestPipeline Do(this IWebApi webApi, HttpMethod method, string path)
        {
            return new HttpRequestPipeline(request =>
            {
                request.Method = method;
                request.RequestUri = webApi.UriForRelativePath(path);

                return request;
            });
        }
        public static IHttpRequestPipeline Get(this IWebApi webApi, string path) => webApi.Do(HttpMethod.Get, path);
        public static IHttpRequestPipeline Put(this IWebApi webApi, string path) => webApi.Do(HttpMethod.Put, path);
        public static IHttpRequestPipeline Post(this IWebApi webApi, string path) => webApi.Do(HttpMethod.Post, path);

        public static IHttpRequestPipeline WithQueryParams(this IHttpRequestPipeline pipeline, IDictionary<string, string> queryParams)
        {
            return new HttpRequestPipeline(request =>
            {
                var transformed = pipeline.Run(request);

                var paramCollection = HttpUtility.ParseQueryString(transformed.RequestUri.Query);

                foreach (var kvp in queryParams)
                {
                    paramCollection.Add(kvp.Key, kvp.Value);
                }

                transformed.RequestUri = new UriBuilder(transformed.RequestUri)
                {
                    Query = paramCollection.ToString(),
                }.Uri;

                return transformed;
            });
        }

        public static IHttpRequestPipeline WithQueryParam(this IHttpRequestPipeline pipeline, string name, string value) =>
            pipeline.WithQueryParams(new Dictionary<string, string> { { name, value } });

        public static IHttpRequestPipeline WithHeaders(this IHttpRequestPipeline pipeline, IDictionary<string, string> headers)
        {
            return new HttpRequestPipeline(request =>
            {
                var transformed = pipeline.Run(request);

                foreach (var kvp in headers)
                {
                    transformed.Headers.Add(kvp.Key, kvp.Value);
                }

                return transformed;
            });
        }

        public static IHttpRequestPipeline WithHeader(this IHttpRequestPipeline pipeline, string name, string value) =>
            pipeline.WithHeaders(new Dictionary<string, string> { { name, value } });
    }
}
