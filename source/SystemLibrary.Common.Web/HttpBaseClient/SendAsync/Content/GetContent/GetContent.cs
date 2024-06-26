﻿using System.Net.Http;
using System.Text;
using System.Text.Json;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    partial class Content
    {
        internal static HttpContent GetContent(object data, MediaType mediaType, JsonSerializerOptions jsonSerializerOptions)
        {
            HttpContent content = null;

            if (data == null) return content;

            if (data is FormUrlEncodedContent formUrlEncodedContent)
            {
                return formUrlEncodedContent;
            }
            else if (data is HttpContent httpContent)
            {
                return httpContent;
            }
            else if (mediaType != MediaType.multipartFormData && data is byte[] bytes)
            {
                var byteContent = new ByteArrayContent(bytes, 0, bytes.Length);

                if (mediaType != MediaType.None)
                    byteContent.Headers.Add("Content-Type", mediaType.ToValue());

                return byteContent;
            }

            switch (mediaType)
            {
                case MediaType.plain:
                    content = GetBodyPlainText(data, null, mediaType);
                    break;

                case MediaType.json:
                    if (jsonSerializerOptions != null)
                        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;

                    content = GetBodyJson(data, null, jsonSerializerOptions, mediaType);
                    break;

                case MediaType.xwwwformUrlEncoded:
                    content = GetBodyXwwwFormUrlEncoded(data);
                    break;

                case MediaType.multipartFormData:
                    content = GetBodyMultipartFormData(data);
                    break;

                case MediaType.octetStream:
                case MediaType.html:
                case MediaType.css:
                case MediaType.javascript:
                case MediaType.pdf:
                case MediaType.zip:
                    throw new System.Exception("Not yet implemented: " + mediaType);

                //TODO: Implement binary json formatter and mediaType application/bson
                //MediaTypeFormatter bsonFormatter = new BsonMediaTypeFormatter();

                default:
                    if (data is HttpRequestMessage msg)
                    {
                        return msg.Content;
                    }

                    content = new StringContent(data is string ? data as string : data.ToString(), Encoding.UTF8, mediaType.ToValue());
                    break;
            }

            return content;
        }
    }
}
