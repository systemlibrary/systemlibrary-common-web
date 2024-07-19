using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Client
    {
        class RetryHandler : DelegatingHandler
        {
            public bool RetryOnTransientErrors;

            public RetryHandler(bool retryOnTransientErrors, SslIgnoreHandler sslIgnoreHandler) : base(sslIgnoreHandler)
            {
                RetryOnTransientErrors = retryOnTransientErrors;
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (!RetryOnTransientErrors)
                    return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

                HttpResponseMessage response = null;
                try
                {
                    response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Any non-cancel exception occured
                    if (ex is not TaskCanceledException && ex is not OperationCanceledException)
                    {
                        if (IsRequestEligibleForRetry(response, request.Method))
                            throw new RetryHttpRequestException();
                    }

                    // Propagate the exception to TimeoutHandler
                    throw;
                }

                if (IsRequestEligibleForRetry(response, request.Method))
                    throw new RetryHttpRequestException();

                return response;
            }

            bool IsRequestEligibleForRetry(HttpResponseMessage response, HttpMethod method)
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
                    statusCode == System.Net.HttpStatusCode.GatewayTimeout;
            }
        }
    }
}