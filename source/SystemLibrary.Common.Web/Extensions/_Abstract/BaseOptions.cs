﻿namespace SystemLibrary.Common.Web.Extensions;

public abstract class BaseOptions
{
    /// <summary>
    /// Set to true to add services and middleware for razor pages
    /// 
    /// NOTE: This also registers a default media type output formatter, so your application is allowed to serve default mime types, such as: html, css, js, jpg, png, tiff, woff, json, xml, and a few others
    /// </summary>
    public bool UseRazorPages = true;

    /// <summary>
    /// Set to true to add services and middleware for controllers
    /// </summary>
    public bool UseControllers = true;

    /// <summary>
    /// Set to true to add services and middleware for /api/ controllers
    /// </summary>
    public bool UseApiControllers = true;

    /// <summary>
    /// Set to true to add services and middleware for cookie policies
    /// </summary>
    public bool UseCookiePolicy = true;

    /// <summary>
    /// Set to true to add services and middleware for forwarded headers
    /// </summary>
    public bool UseForwardedHeaders = true;

    /// <summary>
    /// Set to true to add services and middleware to use http to https redirection
    /// </summary>
    public bool UseHttpsRedirection = true;

    /// <summary>
    /// Set to true to register services and middleware for the OutputCache in ASPNET
    /// </summary>
    public bool UseOutputCache = true;

    /// <summary>
    /// Set to true to register services and middleware for the OutputCache in ASPNET after the Authentication, so Authentication always triggers before checking output cache
    /// <para>This is the output cache middleware from microsoft, completely different cache than the class Cache in this library</para>
    /// </summary>
    public bool UseOutputCacheAfterAuthentication = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseGzipResponseCompression = true;

    /// <summary>
    /// Set to true to add services and middleware Gzip compression
    /// </summary>
    public bool UseBrotliResponseCompression = false;
}