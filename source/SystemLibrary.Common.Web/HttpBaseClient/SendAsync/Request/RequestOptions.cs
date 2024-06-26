﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    internal class RequestOptions
    {
        public HttpMethod Method;
        public string Url;
        public HttpContent Content;
        public int TimeoutMilliseconds;
        public IDictionary<string, string> Headers;
        public bool ForceNewClient;
        public bool RetryOnceOnRequestCancelled;
        public bool IgnoreSslErrors;
        public CancellationToken CancellationToken;
        public MediaType MediaType;
    }
}
