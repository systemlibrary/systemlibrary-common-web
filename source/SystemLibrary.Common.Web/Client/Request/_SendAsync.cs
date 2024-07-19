using System.Net.Http;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class Request
    {
        internal static async Task<HttpResponseMessage> SendAsync(RequestOptions options)
        {
            var message = CreateHttpRequestMessage(options);

            var client = ClientCached.GetClient(options.Url,
                options.Timeout,
                options.UseRetryPolicy,
                options.ForceNewClient,
                options.IgnoreSslErrors);

            using (message)
                return await client.SendAsync(message, options.CancellationToken).ConfigureAwait(false);
        }
    }
}
