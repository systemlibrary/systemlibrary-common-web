using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Web
{
    partial class HttpBaseClient
    {
        static async Task<T> ReadResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken, JsonSerializerOptions jsonSerializerOptions)
        {
            if (!response.IsSuccessStatusCode)
                ThrowRequestException(response);

            if (response.Content == null) return default;

            var type = typeof(T);

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
                    return bool.TryParse(body.ToString(), out bool value) ? (T)(object)value : default;

                else if (type == SystemType.DateTimeType)
                    return DateTime.TryParse(body.ToString(), out DateTime value) ? (T)(object)value : default;

                else
                    throw new Exception("Type: " + type.Name + " is not yet implemented for method ReadResponseAsync()");
            }

            //TODO: Support XML serialization/deserialization?

            if(jsonSerializerOptions == default)
            {
                jsonSerializerOptions = new JsonSerializerOptions()
                {
                    AllowTrailingCommas = true,
                    IgnoreNullValues = true,
                    PropertyNameCaseInsensitive = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    MaxDepth = 32
                };
            }

            using (response)
                using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new StreamReader(contentStream))
                        return await JsonSerializer.DeserializeAsync<T>(contentStream, jsonSerializerOptions, cancellationToken).ConfigureAwait(false);
        }
    }
}
