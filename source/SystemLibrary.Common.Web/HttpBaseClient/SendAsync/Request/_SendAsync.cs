using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        partial class Request
        {
            static async Task<HttpResponseMessage> SendAsync(RequestOptions options)
            {
                var message = GetMessage(options);

                var client = Client.GetClient(options.Url,
                    options.TimeoutMilliseconds,
                    options.RetryOnceOnRequestCancelled,
                    options.ForceNewClient,
                    options.IgnoreSslErrors);

                using (message)
                {
                    return await client
                        .SendAsync(message, options.CancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
