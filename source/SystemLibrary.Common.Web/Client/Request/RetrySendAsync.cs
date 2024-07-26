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
            var maxRetries = options.UseRetryPolicy ? 3 : 1;

            HttpResponseMessage response = null;
            Exception ex = null;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                ex = null;
                response = null;

                options.Update(retry);

                try
                {
                    response = await SendAsync(options);
                }
                catch(ArgumentException arg)
                {
                    ex = arg;
                    break;
                }
                catch(InvalidOperationException invalid)
                {
                    ex = invalid;
                    break;
                }
                catch(IndexOutOfRangeException index)
                {
                    ex = index;
                    break;
                }
                catch(NullReferenceException noref)
                {
                    ex = noref;
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                }

                if (options.CancellationToken.IsCancellationRequested)
                {
                    ex = new CalleeCancelledRequestException("Callee cancelled request to " + options.Url);
                    break;
                }

                if (!IsEligibleForRetry(options, response, retry))
                {
                    break;
                }

                if (maxRetries > 1 && retry != maxRetries - 1)
                    await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
            }

            return (response, ex);
        }
    }
}
