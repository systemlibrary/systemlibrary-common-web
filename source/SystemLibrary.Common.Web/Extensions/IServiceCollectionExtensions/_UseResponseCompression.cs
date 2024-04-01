using System.IO.Compression;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web.Extensions;

static partial class IServiceCollectionExtensions
{
    static string[] CompressMimeTypes = new string[]
    {
        "text/html",
        "text/html; charset=utf-8",
        "text/css",
        "text/css; charset=utf-8",
        "text/csv",
        "text/csv; charset=utf-8",
        "text/plain",
        "text/plain; charset=utf-8",
        "text/javascript",
        "text/javascript; charset=utf-8",
        "text/xml",

        "font/ttf",
        "font/otf",
        "font/woff2",
        "font/woff",

        "image/svg+xml",
        "image/jpeg",
        "image/gif",
        "image/png",
        "image/tiff",
        "image/webp",
        "image/x-icon",

        "application/x-font-ttf",
        "application/x-font-opentype",

        "application/zip",
        "application/gzip",
        "application/pdf",
        "application/json",
        "application/javascript",
        "application/rss+xml",
        "application/pkcs8",
        "application/xml",

        "video/mp4",
        "video/m4v",
        "video/webm",

    };

    static IServiceCollection UseResponseCompression(this IServiceCollection services)
    {
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

        services.AddResponseCompression(compression =>
        {
            compression.EnableForHttps = true;
            compression.MimeTypes = CompressMimeTypes;
            compression.Providers.Add<BrotliCompressionProvider>();
            compression.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Optimal;
        });

        return services;
    }
}
