using System;
using System.Net.Http;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        class CacheModel
        {
            public HttpClient HttpClientCached { get; set; }
            public DateTime Expires { get; set; }

            public void Dispose()
            {
                HttpClientCached?.Dispose();
                HttpClientCached = null;
            }
        }
    }
}