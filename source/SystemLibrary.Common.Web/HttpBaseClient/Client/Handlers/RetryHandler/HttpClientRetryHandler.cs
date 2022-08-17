using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web
{
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
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { Log.Error("HttpBaseClient: ssl exception occured (invalid or expired), but ignoreSslErrors is set to true, continuing..."); return true; };
                    }
                }

                protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    try
                    {
                        return await base
                            .SendAsync(request, cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (TaskCanceledException)
                    {
                        if (request.Method == HttpMethod.Get ||
                            request.Method == HttpMethod.Head ||
                            request.Method == HttpMethod.Trace)
                            throw new RetryRequestException();

                        throw;
                    }
                }
            }
        }
    }
}