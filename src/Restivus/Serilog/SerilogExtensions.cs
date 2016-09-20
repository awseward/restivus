using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Restivus.Serilog
{
    public static class SerilogExtensions
    {
        public static LoggerConfiguration FilterQueryParamsFromHttpRequestMessage(
            this LoggerConfiguration loggerConfig,
            params string[] queryParamKeys)
        {
            return loggerConfig.Destructure.ByTransforming<HttpRequestMessage>(
                msg => new
                {
                    msg.Method.Method,
                    Uri = msg.RequestUri.FilterQueryParams(queryParamKeys),
                    msg.Headers,
                }
            );
        }
    }
}
