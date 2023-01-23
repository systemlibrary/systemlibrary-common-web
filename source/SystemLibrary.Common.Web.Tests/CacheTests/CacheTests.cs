using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class CacheTests
{
    const string CacheKey = "helloworld";

    [TestMethod]
    public void Get_From_Cache_NotExisting_Success()
    {
        var item = Cache.Get<string>(CacheKey + "1");

        Assert.IsTrue(item == null, "Item is not null");
    }

    [TestMethod]
    public void Add_To_Cache_Success()
    {
        var cached = Cache.Get(() =>
        {
            return "Text";
        },
        cacheKey: CacheKey + "2",
        debug: true);

        Assert.IsTrue(cached == "Text", "Cache did not return the GetItem() value");

        cached = Cache.Get<string>(CacheKey + "2");

        Assert.IsTrue(cached == "Text", "Cached was null");
    }

    [TestMethod]
    public void Add_To_Cache_Auto_CacheKey_Passing_Function_As_GetItem_Success()
    {
        var getItems = () => GetText("Hello", 101, true);

        var cached = Cache.Get(GetItemsFromFunction, debug: true);
        Assert.IsTrue(cached.Is());

        cached = Cache.Get(GetItemsFromFunction);
        Assert.IsTrue(cached.Is());
        Assert.IsTrue(cached.Contains("55") && cached.Contains("World"));

        var cacheKey = "GetItemsFromFunctionSystemLibrary.Common.Web.Tests.CacheTestsSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cachedItem.Contains("55") && cached.Contains("World"));
    }

    static string GetItemsFromFunction()
    {
        string a = "World";
        int b = 55;
        bool c = true;

        return GetText(a, b, c);
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Function_Without_Outside_Vars_Success()
    {
        var getItems = () => GetText("Hello", 123, true);

        var cached = Cache.Get(getItems, debug: true);
        Assert.IsTrue(cached.Is());

        cached = Cache.Get(getItems, debug: true);
        Assert.IsTrue(cached.Is());
        Assert.IsTrue(cached.Contains("123"));

        var cacheKey = "<Auto_Create_CacheKey_By_Passing_Function_Without_Outside_Vars_Success>b__5_0SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cachedItem.Contains("123"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success()
    {
        var cached = Cache.Get(() => GetText("Hello", 111, true));
        Assert.IsTrue(cached.Is());

        var cacheKey = "<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__6_0SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cachedItem.Contains("111"));

        var cached2 = Cache.Get(() => GetText("Hello", 222, true));
        var cacheKey2 = "<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__6_1SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem2 = Cache.Get<string>(cacheKey2);
        Assert.IsTrue(cachedItem2.Is());
        Assert.IsTrue(cachedItem2.Contains("222"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Inline_Lambda_With_Outside_Vars_Vars_Are_Part_Of_CacheKey_Success()
    {
        string a = "Hello";
        int b = 333;
        bool c = true;
        var cached = Cache.Get(() =>
        {
            return GetText(a, b, c);
        });
        Assert.IsTrue(cached.Is());

        var cacheKey = "<Auto_Create_CacheKey_By_Inline_Lambda_With_Outside_Vars_Vars_Are_Part_Of_CacheKey_Success>b__0SystemLibrary.Common.Web.Tests.CacheTests+<>c__DisplayClass7_0System.StringHello333True";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Contains(b.ToString()));
        Assert.IsTrue(cachedItem.Contains(c.ToString()));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Function_With_Outside_Vars_Success()
    {
        var a = "Hello";
        var b = 555;
        var c = true;

        var getItems = () => GetText(a, b, c);

        var cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Contains("555"));

        cached = Cache.Get(getItems);
        Assert.IsTrue(cached.Contains("555"));

        var cacheKey = "<" + nameof(Auto_Create_CacheKey_By_Passing_Function_With_Outside_Vars_Success) + ">b__0SystemLibrary.Common.Web.Tests." + nameof(CacheTests) + "+<>c__DisplayClass8_0System.String" + a + b + c;
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Contains("555"));
    }

    [TestMethod]
    public void Upsert_Lock_Tests()
    {
        var a = 0;
        var b = 0;
        if (Cache.Lock(TimeSpan.FromSeconds(1)))
        {
            a++;
        }
        if (Cache.Lock(TimeSpan.FromSeconds(1)))
        {
            a++;
        }
        if (Cache.Lock())
        {
            b++;
            a++;
        }
        if (Cache.Lock())
        {
            a++;
        }
        if (Cache.Lock())
        {
            a++;
            b++;
        }

        if (Cache.Lock(TimeSpan.FromSeconds(1)))
        {
            a++;
        }
        Assert.IsTrue(a == 2 && b == 1, "A: " + a + ", B: " + b);

        System.Threading.Thread.Sleep(1100);
        if (Cache.Lock(TimeSpan.FromSeconds(1)))
        {
            a++;
        }

        Assert.IsTrue(a == 3 && b == 1, "A: " + a + ", B: " + b);

        if (Cache.Lock(TimeSpan.FromSeconds(1)))
        {
            a++;
        }
        Assert.IsTrue(a == 3 && b == 1, "A: " + a + ", B: " + b);
    }

    static string GetText(string a, int b = 99, bool c = false)
    {
        return a + b + c;
    }
}
