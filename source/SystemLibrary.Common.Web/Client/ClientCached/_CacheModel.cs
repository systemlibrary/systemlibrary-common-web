using System;
using System.Net.Http;

namespace SystemLibrary.Common.Web;

partial class Client
{
    class CacheModel
    {
        public HttpClient CachedClient;
        public DateTime Expires;

        public void Dispose()
        {
            CachedClient?.Dispose();
            CachedClient = null;
        }
    }
}