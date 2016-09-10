using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Restivus
{
    public interface IMiddleware<T>
    {
        T Run(T thing);
        Task<T> RunAsync(T thing);
    }

    public interface IHttpRequestMiddleware : IMiddleware<HttpRequestMessage> { }

    public class HttpRequestMiddleware : IHttpRequestMiddleware
    {
        public HttpRequestMessage Run(HttpRequestMessage thing)
        {
            throw new NotImplementedException();
        }

        public Task<HttpRequestMessage> RunAsync(HttpRequestMessage thing)
        {
            throw new NotImplementedException();
        }
    }
}
