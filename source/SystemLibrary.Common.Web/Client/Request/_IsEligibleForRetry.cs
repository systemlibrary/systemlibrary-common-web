using System.Net;
using System.Net.Http;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class Request
    {
        static bool IsEligibleForRetry(RequestOptions options, HttpResponseMessage response, int retry)
        {
            HttpMethod method = options.Method;
            
            return
                IsResponseEligibleForRetry(response, method) &&
                IsRequestEligibleForRetry(options, method, response?.StatusCode, retry);
        }

        static bool IsRequestEligibleForRetry(RequestOptions options, HttpMethod method, HttpStatusCode? responseStatusCode, int retry)
        {
            // FileRequest: 1 retry
            if (options.Url.IsFile()) return retry == 0;

            if (method == HttpMethod.Delete || method == HttpMethod.Put)
            {
                // DELETE, PUT: 0 retries
                return false;
            }
            else if (method == HttpMethod.Get)
            {
                // GET 404: 1 retry
                if (responseStatusCode == HttpStatusCode.NotFound) return retry == 0;

                // GET: 2 retries
                return true;
            }

            // OPTION, PATCH, HEAD, CONNECT, TRACE, POST: 1 retry
            return retry == 0;
        }

        static bool IsResponseEligibleForRetry(HttpResponseMessage response, HttpMethod method)
        {
            var statusCode = response?.StatusCode;

            if (statusCode == null)
            {
                return true;
            }

            return
                statusCode == HttpStatusCode.InternalServerError ||
                statusCode == HttpStatusCode.BadGateway ||
                statusCode == HttpStatusCode.GatewayTimeout;
        }
    }
}
