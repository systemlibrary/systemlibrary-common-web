using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Client
    {
        class TimeoutHandler : DelegatingHandler
        {
            TimeSpan RequestTimeoutSpan;

            public TimeoutHandler(int timeoutMilliseconds, RetryHandler retryRequestHandler) : base(retryRequestHandler)
            {
                if (timeoutMilliseconds > 0)
                    RequestTimeoutSpan = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                else
                    RequestTimeoutSpan = TimeSpan.MaxValue;
            }

            protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                using (var source = GetTimeoutCancellationToken(cancellationToken))
                {
                    try
                    {
                        return await base
                            .SendAsync(request, source.Token)
                            .ConfigureAwait(false);
                    }
                    catch (TaskCanceledException ex)
                    {
                        // This should either propagate the error OR
                        // throw "Callee cancelled request exceptions" 
                        // or "retry exceptions"...
                        // or "Timeout Exceptions"
                        // Need to grab "TaskCanceledException" and "OperationCanceledException

                        // CancelAfter() occured
                        if (ex.CancellationToken.IsCancellationRequested)
                        {
                            if(InnerHandler is RetryHandler retryHandler)
                            {
                                // Retry is configured, propagate a new retry request
                                if (retryHandler.RetryOnTransientErrors)
                                {
                                    throw new RetryHttpRequestException();
                                }
                            }
                        }

                        if (InnerHandler is RetryHandler retryHandler2)
                        {
                            // Retry is configured, propagate a new retry request
                            if (retryHandler2.RetryOnTransientErrors)
                            {
                                throw new RetryHttpRequestException();
                            }
                        }

                        throw new CalleeCancelledRequestException("Callee cancelled the request threw an operation cancel trigger through a cancellation token (likely): " + request.RequestUri.AbsoluteUri + ".");
                    }

                    // An external cancellation token was passed and "CancelAfter()" triggered
                    catch(OperationCanceledException calleeOperationCancelledException)
                    {
                        throw new TimeoutException("Request to " + request.RequestUri.AbsoluteUri + " timed out after the configured timeout of: " + RequestTimeoutSpan.ToString(@"ss\.fff") + " seconds, or was cancelled by server. ");
                    }
                }
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
                return
                    RequestTimeoutSpan != TimeSpan.MinValue &&
                    RequestTimeoutSpan != TimeSpan.MaxValue &&
                    RequestTimeoutSpan != Timeout.InfiniteTimeSpan;
            }
        }
    }
}