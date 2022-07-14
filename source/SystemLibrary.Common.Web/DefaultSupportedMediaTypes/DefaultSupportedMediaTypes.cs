using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Web;

internal class DefaultSupportedMediaTypes : StringOutputFormatter
{
    internal DefaultSupportedMediaTypes()
    {
        SupportedMediaTypes.Add("text/html");
        SupportedMediaTypes.Add("text/css");
        SupportedMediaTypes.Add("text/csv");
        SupportedMediaTypes.Add("text/plain");
        SupportedMediaTypes.Add("text/javascript");

        SupportedMediaTypes.Add("font/ttf");
        SupportedMediaTypes.Add("font/otf");
        SupportedMediaTypes.Add("font/woff2");
        SupportedMediaTypes.Add("font/woff");

        SupportedMediaTypes.Add("image/svg+xml");
        SupportedMediaTypes.Add("image/jpeg");
        SupportedMediaTypes.Add("image/gif");
        SupportedMediaTypes.Add("image/png");
        SupportedMediaTypes.Add("image/tiff");
        SupportedMediaTypes.Add("image/webp");
        SupportedMediaTypes.Add("image/x-icon");

        SupportedMediaTypes.Add("application/x-font-ttf");
        SupportedMediaTypes.Add("application/x-font-opentype");

        SupportedMediaTypes.Add("application/octet-stream");
        SupportedMediaTypes.Add("application/zip");
        SupportedMediaTypes.Add("application/gzip");
        SupportedMediaTypes.Add("application/pdf");
        SupportedMediaTypes.Add("application/json");
        SupportedMediaTypes.Add("application/rss+xml");

        SupportedMediaTypes.Add("video/mp4");
        SupportedMediaTypes.Add("video/m4v");
        SupportedMediaTypes.Add("video/webm");
    }
}
