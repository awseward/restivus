using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Restivus;

namespace Serilog.Restivus
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration FilterRequestQueryParams(
            this LoggerConfiguration loggerConfig,
            params string[] queryParamKeys)
        {
            return loggerConfig.Destructure.ByTransforming<HttpRequestMessage>(
                request => new
                {
                    request.Method.Method,
                    Uri = request.RequestUri.FilterQueryParams(queryParamKeys),
                    Headers = request.Headers._SlightlySimplified(),
                }
            );
        }

        public static LoggerConfiguration DestructureHttpResponseMessages(
            this LoggerConfiguration loggerConfig)
        {
            return loggerConfig.Destructure.ByTransforming<HttpResponseMessage>(
                response => new
                {
                    Status = (int) response.StatusCode,
                    response.RequestMessage,
                    Headers = response.Headers._SlightlySimplified(),
                }
            );
        }

        static IDictionary<string, string> _SlightlySimplified(this HttpHeaders headers) =>
            headers.ToDictionary(
                kvp => kvp.Key,
                kvp => string.Join(", ", kvp.Value)
            );
    }
}
