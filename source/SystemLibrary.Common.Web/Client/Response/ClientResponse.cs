using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Web;

/// <summary>
/// The response that all Client methods returns
/// <para>Note that a response statuscode might be 200 OK, but the IsSuccess might be false in scenarios where no response were returned</para>
/// </summary>
/// <typeparam name="T">T can be a string that you later can convert to json, or it can be a class, or a list of a class that will automatically be converted from json, assuming json response</typeparam>
public partial class ClientResponse<T> : ClientResponse, IDisposable
{
    internal ClientResponse(HttpResponseMessage response, T data, string reason = null)
    {
        Response = response;
        Data = data;
        Reason = reason;
        if (reason != null)
        {
            if (response == null)
            {
                if (reason.StartsWith("No such host"))
                    Code = (HttpStatusCode)599;
                else
                    Code = HttpStatusCode.BadGateway;
            }
        }
    }

    /// <summary>
    /// Pass in data that is set on as the 'data' with a response of status code 500
    /// <para>Use in your own 'BaseClient' if you want to sent some specific message or data upwards</para>
    /// </summary>
    public ClientResponse(T defaultData = default)
    {
        Data = defaultData;
        Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        Response.ReasonPhrase = HttpStatusCode.InternalServerError.ToString();
    }

    /// <summary>
    /// Pass in data that is set on as the 'data' with a response with a status code of your own choice
    /// </summary>
    public ClientResponse(T defaultData, HttpStatusCode statusCode)
    {
        Data = defaultData;
        Response = new HttpResponseMessage(statusCode);
        Response.ReasonPhrase = statusCode.ToString();
    }

    [JsonIgnore]
    string Reason;

    [JsonIgnore]
    HttpStatusCode Code = HttpStatusCode.OK;

    public T Data { get; }

    public HttpStatusCode StatusCode => Response?.StatusCode ?? Code;
    public string Message => Response?.ReasonPhrase ?? Reason;

    public void Dispose()
    {
        if (Response != null)
        {
            try
            {
                Response.Dispose();
            }
            catch
            {
            }
        }
    }
}

/// <summary>
/// Base class of a ClientResponse 
/// <para>- Contains the HttpResponseMessage itself </para>
/// Used when you do not want to return 'object' nor generic type, but you want to be somewhat clear in what object is returned
/// </summary>
public class ClientResponse
{
    [JsonIgnore]
    public HttpResponseMessage Response { get; internal set; }
    public bool IsSuccess => Response?.IsSuccessStatusCode == true;

}