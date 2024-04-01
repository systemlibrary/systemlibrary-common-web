using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Request
    {
        internal static async Task<HttpResponseMessage> SendRequestAsync(RequestOptions options)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await SendAsync(options).ConfigureAwait(false);
            }
            catch (RetryRequestException retry)
            {
                if (options.CancellationToken != null && options.CancellationToken.IsCancellationRequested)
                    throw new Exception("Cancelled: " + retry?.InnerException?.Message + ". " + retry?.InnerException?.StackTrace);

                try
                {
                    options.ForceNewClient = true;

                    var timeoutSeconds = AppSettings.Current.SystemLibraryCommonWeb.HttpBaseClient.RetryRequestTimeoutSeconds;

                    options.TimeoutMilliseconds = timeoutSeconds < 1 ? 10000 : timeoutSeconds * 1000;

                    if (options.TimeoutMilliseconds > 120000)
                        options.TimeoutMilliseconds = 120000;

                    response = await SendAsync(options).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    throw new RetryRequestException(response?.StatusCode + ": " + response?.ReasonPhrase + " error on retrying " + options.Url + " timeout: " + options.TimeoutMilliseconds + " ms", ex);
                }
            }

            return response;
        }
    }
}