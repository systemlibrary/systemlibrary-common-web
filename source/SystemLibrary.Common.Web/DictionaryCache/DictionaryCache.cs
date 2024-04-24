using System.Collections.Concurrent;
using System.Reflection;

namespace SystemLibrary.Common.Web;

internal static class DictionaryCache
{
    internal static ConcurrentDictionary<int, FieldInfo[]> CacheTypeFieldsCache;

    static DictionaryCache()
    {
        CacheTypeFieldsCache = new ConcurrentDictionary<int, FieldInfo[]>();
    }
}
