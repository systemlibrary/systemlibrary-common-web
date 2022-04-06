using System;

namespace SystemLibrary.Common.Web
{
    /// <summary>
    /// A retry request exception is thrown if:
    /// - "retryOnceOnRequestCancelled" is set to True, and if both the normal request and the retry request errors, this exception is thrown to the callee (you)
    /// - a retry request exception can only occur on GET, HEAD or OPTION requests
    /// 
    /// If retryOnceOnRequestCancelled is False or if request method is POST, PUT or DELETE, this exception will never occur for the callee (you)
    /// 
    /// Note: A retry request gets a fixed timeout of 10 seconds to finish the request
    /// </summary>
    public class RetryRequestException : Exception
    {
        public RetryRequestException(string message = null, Exception inner = null) : base(message, inner)
        {
        }
    }
}
