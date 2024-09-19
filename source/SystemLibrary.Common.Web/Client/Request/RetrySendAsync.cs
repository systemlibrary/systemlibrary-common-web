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
            var maxRetries = options.UseRetryPolicy ? 3 : 2;

            HttpResponseMessage response = null;
            Exception ex = null;

            for (int retry = 0; retry < maxRetries; retry++)
            {
                options.Update(retry);
                try
                {
                    response = await SendAsync(options);
                }
                catch (ArgumentException arg)
                {
                    ex = arg;
                    break;
                }
                catch (InvalidOperationException invalid)
                {
                    ex = invalid;
                    break;
                }
                catch (IndexOutOfRangeException index)
                {
                    ex = index;
                    break;
                }
                catch (NullReferenceException noref)
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
                    ex = new CalleeCancelledRequestException("Callee cancelled request to " + options.Url, ex);
                    break;
                }

                if (retry != maxRetries - 1)
                {
                    if (!IsEligibleForRetry(options, response, retry, ex))
                    {
                        break;
                    }

                    Debug.Log("Retry count: " + (retry + 1) + " " + options.Url + ": " + response?.StatusCode);

                    ex = null;
                    response = null;
                    await Task.Delay(TimeSpan.FromMilliseconds(666)).ConfigureAwait(false);
                }
            }

            return (response, ex);
        }
    }
}
