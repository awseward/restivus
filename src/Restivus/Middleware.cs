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
    }

    public interface IHttpRequestMiddleware : IMiddleware<HttpRequestMessage> { }

    public class HttpRequestMiddleware : IHttpRequestMiddleware
    {
        public HttpRequestMiddleware(Func<HttpRequestMessage, HttpRequestMessage> run)
        {
            _run = run.AsNoOpIfNull();
        }

        public HttpRequestMiddleware(Action<HttpRequestMessage> run)
            : this(run.AsFluent()) { }

        readonly Func<HttpRequestMessage, HttpRequestMessage> _run;

        public HttpRequestMessage Run(HttpRequestMessage thing) => _run(thing);
    }
}
