﻿using System;
using System.Net;
using System.Net.Http;

namespace SystemLibrary.Common.Web;

/// <summary>
/// The response that all Client methods returns
/// </summary>
/// <typeparam name="T">T can be a string that you later can convert to json, or it can be a class, or a list of a class that will automatically be converted from json, assuming json response</typeparam>
public partial class ClientResponse<T> : IDisposable
{
    internal ClientResponse(HttpResponseMessage response, T data)
    {
        Response = response;
        Data = data;
    }

    public HttpResponseMessage Response { get; }
    public T Data { get; }
    public HttpStatusCode StatusCode => Response?.StatusCode ?? HttpStatusCode.OK;
    public bool IsSuccess => Response?.IsSuccessStatusCode == true;
    public string Message => Response?.ReasonPhrase;

    public void Dispose()
    {
        if(Response != null)
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
