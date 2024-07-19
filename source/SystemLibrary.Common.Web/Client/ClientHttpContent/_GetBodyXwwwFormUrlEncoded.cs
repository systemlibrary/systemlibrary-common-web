using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class Client
{
    partial class ClientHttpContent
    {
        static HttpContent GetBodyXwwwFormUrlEncoded(object data)
        {
            if (data is List<KeyValuePair<string, string>> keyValuePairCollection)
            {
                return new FormUrlEncodedContent(keyValuePairCollection);
            }
            else if (data is IDictionary dictionary)
            {
                var keyValuePairs = new List<KeyValuePair<string, string>>();

                foreach (DictionaryEntry keyValue in dictionary)
                    keyValuePairs.Add(new KeyValuePair<string, string>(keyValue.Key.ToString(), keyValue.Value?.ToString()));

                return new FormUrlEncodedContent(keyValuePairs);
            }
            else if (data is ExpandoObject expando)
            {
                throw new System.Exception("Expando is currently not fully implemented in GetBodyXwwwFormUrlEncoded()");
            }
            else if (data is string text)
            {
                if (text.Contains("&") && text.Contains("="))
                {
                    var keyValues = new List<KeyValuePair<string, string>>();

                    var inputs = text.Split('&');
                    foreach (var input in inputs)
                    {
                        if (input.IsNot()) continue;

                        var keyValue = input.Split('=');

                        if (keyValue.IsNot()) continue;

                        keyValues.Add(new KeyValuePair<string, string>(keyValue[0], keyValue.Length > 1 ? keyValue[1] : null));
                    }

                    return new FormUrlEncodedContent(keyValues);
                }
                return new StringContent(text, Encoding.UTF8);
            }
            else if (data is byte[] bytes)
            {
                return new ByteArrayContent(bytes, 0, bytes.Length);
            }
            else if (data.GetType().IsClass)
                throw new System.Exception("Class to wwwformurlencoded string is not currently fully implemented  in GetBodyXwwwFormUrlEncoded()");

            throw new System.Exception("x-www-form-urlencoded media type requires data sent to be either: List<KeyValuePair<string, string>> or IDictionary, or a string or a byte[] or as null, but then the URL you pass into the method must already contain the key/values in the url");
        }
    }
}