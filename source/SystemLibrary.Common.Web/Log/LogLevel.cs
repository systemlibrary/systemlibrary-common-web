using SystemLibrary.Common.Net.Attributes;
public enum LogLevel
{
    Trace = 0,

    Information = 1,

    Debug,

    Warning,

    [EnumValue("Critical")]
    Error,

    None = 999
}
