using Microsoft.AspNetCore.Mvc.Formatters;

namespace SystemLibrary.Common.Web.Extensions
{
    internal class DefaultSupportedMediaTypes : StringOutputFormatter
    {
        internal DefaultSupportedMediaTypes()
        {
            SupportedMediaTypes.Add("text/html");
            SupportedMediaTypes.Add("text/css");
            SupportedMediaTypes.Add("text/csv");
            SupportedMediaTypes.Add("text/plain");
            SupportedMediaTypes.Add("text/script");
            SupportedMediaTypes.Add("text/javascript");

            SupportedMediaTypes.Add("x-font-ttf");
            SupportedMediaTypes.Add("font/ttf");
            SupportedMediaTypes.Add("font/woff2");
            SupportedMediaTypes.Add("font/woff");

            SupportedMediaTypes.Add("image/svg+xml");
            SupportedMediaTypes.Add("image/jpeg");
            SupportedMediaTypes.Add("image/png");
            SupportedMediaTypes.Add("image/tiff");
            SupportedMediaTypes.Add("image/webp");
            
            SupportedMediaTypes.Add("application/octet-stream");
            SupportedMediaTypes.Add("application/zip");
            SupportedMediaTypes.Add("application/pdf");
            SupportedMediaTypes.Add("application/json");

            SupportedMediaTypes.Add("video/mp4");
        }
    }
}