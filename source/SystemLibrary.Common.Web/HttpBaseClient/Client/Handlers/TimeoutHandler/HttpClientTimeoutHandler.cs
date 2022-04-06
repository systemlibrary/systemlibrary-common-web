using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Client
        {
            class HttpClientTimeoutHandler : DelegatingHandler
            {
                TimeSpan RequestTimeoutSpan;

                public HttpClientTimeoutHandler(int timeoutMilliseconds, HttpClientRetryHandler retryRequestHandler = null, bool ignoreSslErrors = false)
                {
                    if (timeoutMilliseconds > 0)
                        RequestTimeoutSpan = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    if (retryRequestHandler != null)
                        InnerHandler = retryRequestHandler;
                    else
                    {
                        InnerHandler = new HttpClientHandler();
                        if (ignoreSslErrors && InnerHandler is HttpClientHandler handler)
                        {
                            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                        }
                    }
                }

                protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    try
                    {
                        using (var source = GetTimeoutCancellationToken(cancellationToken))
                        {
                            return await base
                                .SendAsync(request, source.Token)
                                .ConfigureAwait(false);
                        }
                    }
                    catch (OperationCanceledException operationCancelled)  //Or use the TaskCanceledException
                    {
                        if (ClientRequestedCancellation(cancellationToken))
                            throw new CalleeCancelledRequestException("Request to " + request.RequestUri.AbsoluteUri + " was cancelled by the callee", operationCancelled);

                        throw new TimeoutException("Request to " + request.RequestUri.AbsoluteUri + " timed out after the configured timeout of: " + RequestTimeoutSpan.ToString(@"ss\.fff") + " seconds, or was cancelled by server. " + operationCancelled.Message);
                    }
                    catch (RetryRequestException)
                    {
                        if(ClientRequestedCancellation(cancellationToken))
                            throw new CalleeCancelledRequestException("Callee cancelled the request, not retrying against " + request.RequestUri.AbsoluteUri);

                        throw;
                    }
                }

                static bool ClientRequestedCancellation(CancellationToken cancellationToken)
                {
                    return cancellationToken != null && cancellationToken.IsCancellationRequested;
                }

                CancellationTokenSource GetTimeoutCancellationToken(CancellationToken cancellationToken)
                {
                    var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                    if (IsTimeoutRegistered())
                        cancellationSource.CancelAfter(RequestTimeoutSpan);

                    return cancellationSource;
                }

                bool IsTimeoutRegistered()
                {
                    return RequestTimeoutSpan != null &&
                        RequestTimeoutSpan != TimeSpan.MinValue &&
                        RequestTimeoutSpan != TimeSpan.MaxValue &&
                        RequestTimeoutSpan != Timeout.InfiniteTimeSpan;
                }
            }
        }
    }
}