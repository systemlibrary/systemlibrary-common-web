using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web;

partial class HttpBaseClient
{
    static async Task<T> ReadResponseAsync<T>(string url, HttpResponseMessage response, CancellationToken cancellationToken, JsonSerializerOptions jsonSerializerOptions, bool throwOnUnsuccessfulStatusCode)
    {
        if (throwOnUnsuccessfulStatusCode && !response.IsSuccessStatusCode)
            ThrowRequestException(url, response);

        if (response.Content == null) return default;

        var type = typeof(T);

        if (type == typeof(HttpResponseMessage))
        {
            return (T)(object)response;
        }

        //TODO: Strings should be read as a stream to then be simply returned, avoiding boxing and its prolly faster (measure it?)
        if (type.IsValueType || type == SystemType.StringType)
        {
            var body = await GetResponseBodyAsync(response).ConfigureAwait(false);

            response.Dispose();

            if (body == null)
                return default;

            if (type == SystemType.StringType)
                return (T)(object)body;

            else if (type == SystemType.IntType)
                return (T)(object)Convert.ToInt32(body);

            else if (type == SystemType.BoolType)
                return bool.TryParse(body, out bool value) ? (T)(object)value : default;

            else if (type == SystemType.DateTimeType)
                return (T)(object)body.ToDateTime();

            else if (type == SystemType.DateTimeOffsetType)
                return (T)(object)body.ToDateTimeOffset();

            else
                throw new Exception("Type: " + type.Name + " is not yet implemented for method ReadResponseAsync()");
        }

        //TODO: Support XML serialization/deserialization?

        using (response)
        using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            //using (var streamReader = new StreamReader(contentStream))
            return await JsonSerializer.DeserializeAsync<T>(contentStream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
