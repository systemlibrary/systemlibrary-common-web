using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class Client
{
    class TimeoutHandler : DelegatingHandler
    {
        TimeSpan RequestTimeoutSpan;

        public TimeoutHandler(int timeoutMilliseconds, SocketsHttpHandler innerHandler) : base(innerHandler)
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
                return await base
                    .SendAsync(request, source.Token)
                    .ConfigureAwait(false);
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
                RequestTimeoutSpan != System.Threading.Timeout.InfiniteTimeSpan;
        }
    }
}