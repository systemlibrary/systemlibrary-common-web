using System.Net;
using System.Net.Http;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class Request
    {
        /// <summary>
        /// Request is eligible for a retry if status code is 502 or 504 and url is not a file request
        /// UseRetryPolicy: 
        /// False: retries up to 1 time on GET and POST
        /// True: same as 'false' but adds:
        /// - retries up to 2 times if response is null
        /// - retries once on 404 GET
        /// - retries once on 500 GET, POST
        /// - retries once on file requests
        /// - retries once on OPTION, PATCH, HEAD, CONNECT, TRACE
        /// - retries up to 2 times on 502, 504 on non file requests
        /// </summary>
        static bool IsEligibleForRetry(RequestOptions options, HttpResponseMessage response, int retry)
        {
            if(!options.UseRetryPolicy)
            {
                if(response != null)
                {
                    if (options.Method != HttpMethod.Get && options.Method != HttpMethod.Post && !options.Url.IsFile())
                        return false;

                    return 
                        response.StatusCode == HttpStatusCode.BadGateway ||
                        response.StatusCode == HttpStatusCode.GatewayTimeout;
                }
                
                return false;
            }

            HttpMethod method = options.Method;

            return
                IsResponseEligibleForRetry(response, method) &&
                IsRequestEligibleForRetry(options, method, response?.StatusCode, retry);
        }

        static bool IsRequestEligibleForRetry(RequestOptions options, HttpMethod method, HttpStatusCode? responseStatusCode, int retry)
        {
            if (responseStatusCode == HttpStatusCode.InternalServerError)
            {
                // FileRequest: 0 retries on 500
                if (options.Url.IsFile()) return false;

                // GET POST: 1 retry on 500
                return (method == HttpMethod.Get || method == HttpMethod.Post) && retry == 0;
            }

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
