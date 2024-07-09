using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Request
    {
        static int? _RetryTimeoutMs;

        static int RetryTimeoutMs
        {
            get
            {
                if(_RetryTimeoutMs == null)
                {
                    var temp = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.RetryRequestTimeoutMs;

                    _RetryTimeoutMs = temp < 100 ? 100 : temp;
                }
                return _RetryTimeoutMs.Value;
            }
        }

        internal static async Task<HttpResponseMessage> SendRequestAsync(RequestOptions options)
        {
            var timeoutms = options.TimeoutMilliseconds;

            try
            {
                return await SendAsync(options).ConfigureAwait(false);
            }
            catch (RetryHttpRequestException retry)
            {
                ThrowIfRequestIsCancelled(retry, options, retry);

                // First retry request 750ms sleep before retrying with a custom timeout
                await Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);

                try
                {
                    options.ForceNewClient = true;

                    options.TimeoutMilliseconds = RetryTimeoutMs;

                    return await SendAsync(options).ConfigureAwait(false);
                }
                catch (RetryHttpRequestException retry2)
                {
                    ThrowIfRequestIsCancelled(retry, options, retry2);

                    // Retry again, but sleep 2 seconds then retry
                    await Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(false);
                    options.ForceNewClient = true;
                    options.TimeoutMilliseconds = 5000;
                    HttpResponseMessage response = null;
                    try
                    {
                        response = await SendAsync(options).ConfigureAwait(false);
                    }
                    catch(Exception ex)
                    {
                        throw new RetryHttpRequestException($"{response?.StatusCode}: {response?.ReasonPhrase} error retrying twice against ({options.Method}) {options.Url} with timeout: {timeoutms}ms", ex);
                    }

                    return response;
                }
            }
        }

        static void ThrowIfRequestIsCancelled(Exception ex, RequestOptions options, RetryHttpRequestException retry)
        {
            // NOTE: The handler should never throw RetryException so this shold not occur, but for safety
            if (!options.RetryOnTransientErrors)
                throw new Exception("Error (" + options.Method + ") " + options.Url + " with timeout " + options.TimeoutMilliseconds, ex);

            if (options.CancellationToken.IsCancellationRequested)
                throw new Exception("Cancelled: " + options.Url + ", message: "+ retry?.InnerException?.Message + ". " + retry?.InnerException?.StackTrace);
        }
    }
}