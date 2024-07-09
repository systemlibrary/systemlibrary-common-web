using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Client
    {
        class HttpClientRetryHandler : DelegatingHandler
        {
            public HttpClientRetryHandler(bool ignoreSslErrors)
            {
                InnerHandler = new HttpClientHandler();
                if (ignoreSslErrors && InnerHandler is HttpClientHandler handler)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (errors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
                        {
                            Log.Warning("HttpBaseClient: SslPolicy error occured, " + errors + ". Usually invalid or expired. IgnoreSslErrors is set to 'true' so continuing...");
                        }
                        return true;
                    };
                }
            }

            static bool IsRequestEligibleForRetry(HttpResponseMessage response, HttpMethod method)
            {
                var statusCode = response?.StatusCode;

                if (method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Trace)
                {
                    return statusCode == null ||
                            statusCode == System.Net.HttpStatusCode.BadGateway ||
                            statusCode == System.Net.HttpStatusCode.GatewayTimeout ||
                            statusCode == System.Net.HttpStatusCode.RequestTimeout;
                }

                if(method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Delete)
                {
                    return statusCode == null ||
                        statusCode == System.Net.HttpStatusCode.BadGateway ||
                        statusCode == System.Net.HttpStatusCode.GatewayTimeout;
                }

                return false;
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await base
                        .SendAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException)
                    {
                        if(request.Method == HttpMethod.Get || request.Method == HttpMethod.Trace)
                            throw new RetryHttpRequestException();
                    }

                    if (IsRequestEligibleForRetry(response, request.Method))
                        throw new RetryHttpRequestException();

                    throw;
                }

                if (IsRequestEligibleForRetry(response, request.Method))
                    throw new RetryHttpRequestException();

                return response;
            }
        }
    }
}