using System.Net.Http;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Client
    {
        class SslIgnoreHandler : DelegatingHandler
        {
            public SslIgnoreHandler(bool ignoreSslErrors)
            {
                InnerHandler = new HttpClientHandler();
                if (ignoreSslErrors && InnerHandler is HttpClientHandler ignoreSslHandler)
                {
                    ignoreSslHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
                        if (errors == System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch ||
                            errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNotAvailable)
                        {
                            Log.Warning("HttpBaseClient: SslPolicy error occured, " + errors + ". Usually invalid or expired. IgnoreSslErrors is set to 'true' so continuing...");
                        }
                        return true;
                    };
                }
            }
        }
    }
}