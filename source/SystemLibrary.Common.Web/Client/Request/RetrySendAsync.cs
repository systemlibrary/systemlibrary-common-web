using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class Request
    {
        internal static async Task<(HttpResponseMessage, Exception)> RetrySendAsync(RequestOptions options)
        {
            var retries = options.UseRetryPolicy ? 3 : 1;

            HttpResponseMessage response = null;
            Exception ex = null;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    if(i == 1)
                    {
                        options.Timeout = options.RetryTimeout;
                        options.ForceNewClient = true;
                    }
                    else if(i == 2)
                    {
                        options.Timeout = 5000;
                        options.ForceNewClient = true;
                    }

                    response = await SendAsync(options);

                    if (options.CancellationToken.IsCancellationRequested)
                    {
                        ex = new CalleeCancelledRequestException("Callee cancelled request to " + options.Url);
                        break;
                    }

                    if (!IsResponseEligibleForRetry(response, options.Method))
                    {
                        return (response, null);
                    }
                }
                catch(Exception e)
                {
                    ex = e;
                }

                if(retries > 1)
                    await Task.Delay(TimeSpan.FromMilliseconds(750)).ConfigureAwait(false);

                if (options.CancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            return (response, ex);
        }

        static bool IsResponseEligibleForRetry(HttpResponseMessage response, HttpMethod method)
        {
            var statusCode = response?.StatusCode;

            if (method == HttpMethod.Head ||
                method == HttpMethod.Connect)
            {
                return statusCode == null ||
                    statusCode == System.Net.HttpStatusCode.NotFound ||
                    statusCode == System.Net.HttpStatusCode.BadGateway ||
                    statusCode == System.Net.HttpStatusCode.InternalServerError ||
                    statusCode == System.Net.HttpStatusCode.GatewayTimeout ||
                    statusCode == System.Net.HttpStatusCode.RequestTimeout;
            }

            if (method == HttpMethod.Get ||
                method == HttpMethod.Trace ||
                method == HttpMethod.Options)
            {
                return statusCode == null ||
                    statusCode == System.Net.HttpStatusCode.BadGateway ||
                    statusCode == System.Net.HttpStatusCode.InternalServerError ||
                    statusCode == System.Net.HttpStatusCode.GatewayTimeout ||
                    statusCode == System.Net.HttpStatusCode.RequestTimeout;
            }

            return statusCode == null ||
                statusCode == System.Net.HttpStatusCode.BadGateway ||
                statusCode == System.Net.HttpStatusCode.InternalServerError ||
                statusCode == System.Net.HttpStatusCode.GatewayTimeout;
        }
    }
}
