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

                if(response?.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                    response?.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
                {
                    if (options.Method == HttpMethod.Get || options.Method == HttpMethod.Head)
                    {
                        options.ForceNewClient = true;

                        options.TimeoutMilliseconds = 6000;

                        response = await SendAsync(options).ConfigureAwait(false);
                    }
                }
            }
            catch (System.Net.Sockets.SocketException se)
            {
                if (options.CancellationToken != null && options.CancellationToken.IsCancellationRequested)
                    throw new Exception("Cancelled: " + se?.InnerException?.Message + ". " + se?.InnerException?.StackTrace);

                if (se.Message.Contains("forcibly closed by") || se?.InnerException?.Message.Contains("forcibly closed by") == true)
                {
                    if (options.CancellationToken != null && options.CancellationToken.IsCancellationRequested)
                        throw new Exception("Cancelled: " + se?.InnerException?.Message + ". " + se?.InnerException?.StackTrace);

                    try
                    {
                        options.ForceNewClient = true;

                        options.TimeoutMilliseconds = 10000;

                        response = await SendAsync(options).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(response?.StatusCode + ": " + response?.ReasonPhrase + " error on retrying " + options.Url + " timeout: " + options.TimeoutMilliseconds + " ms", se);
                    }
                }
            }
            catch (HttpRequestException hre)
            {
                if (options.CancellationToken != null && options.CancellationToken.IsCancellationRequested)
                    throw new Exception("Cancelled: " + hre?.InnerException?.Message + ". " + hre?.InnerException?.StackTrace);

                if (hre.Message.Contains("forcibly closed by") || hre?.InnerException?.Message.Contains("forcibly closed by") == true)
                {
                    try
                    {
                        options.ForceNewClient = true;

                        options.TimeoutMilliseconds = 10000;

                        response = await SendAsync(options).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(response?.StatusCode + ": " + response?.ReasonPhrase + " error on retrying " + options.Url + " timeout: " + options.TimeoutMilliseconds + " ms", hre);
                    }
                }
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