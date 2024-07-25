namespace SystemLibrary.Common.Web;

partial class Client
{
    bool IsEligibleForCircuitBreakerPolicy(RequestOptions options)
    {
        if (!UseCircuitBreakerPolicy) return false;

        if (options.MediaType == MediaType.html ||
            options.MediaType == MediaType.javascript ||
            options.MediaType == MediaType.octetStream ||
            options.MediaType == MediaType.css) return false;

        return !options.Url.IsFile();
    }
}
