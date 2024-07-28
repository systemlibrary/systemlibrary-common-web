using System;
using System.Runtime.Serialization;

namespace SystemLibrary.Common.Web
{
    [Serializable]
    internal class HttpRequestExxception : Exception
    {
        public HttpRequestExxception()
        {
        }

        public HttpRequestExxception(string message) : base(message)
        {
        }

        public HttpRequestExxception(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpRequestExxception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}