using System.Collections.Concurrent;
using System.Reflection;

namespace SystemLibrary.Common.Web;

internal static class Dictionaries
{
    internal static ConcurrentDictionary<int, FieldInfo[]> GenerateCacheKeyFields;
    internal static ConcurrentDictionary<int, PropertyInfo[]> GenerateCacheKeyValueTypeProperties;
    internal static ConcurrentDictionary<int, FieldInfo[]> GenerateCacheKeyValueTypeFields;

    static Dictionaries()
    {
        GenerateCacheKeyFields = new ConcurrentDictionary<int, FieldInfo[]>();
        GenerateCacheKeyValueTypeProperties = new ConcurrentDictionary<int, PropertyInfo[]>();
        GenerateCacheKeyValueTypeFields = new ConcurrentDictionary<int, FieldInfo[]>();
    }
}
