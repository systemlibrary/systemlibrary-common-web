using SystemLibrary.Common.Net.Attributes;

namespace SystemLibrary.Common.Web
{
    /// <summary>
    /// An enum of various media types that can be sent to the HttpBaseClient request methods
    /// 
    /// Not all of them have been implemented yet though
    /// </summary>
    public enum MediaType
    {
        [EnumValue("application/json")]
        json,

        [EnumValue("application/x-www-form-urlencoded")]
        xwwwformUrlEncoded,

        [EnumValue("text/plain")]
        textplain,

        [EnumValue("multipart/form-data")]
        multipartFormData,
        
        [EnumValue("application/octet-stream")]
        octetStream,

        [EnumValue("text/html")]
        html,

        [EnumValue("text/css")]
        css,

        [EnumValue("text/javascript")]
        javascript,

        [EnumValue("application/pdf")]
        pdf,
        
        [EnumValue("application/zip")]
        zip,

        [EnumValue("")]
        None,
    }
}
