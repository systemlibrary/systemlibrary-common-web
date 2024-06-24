using System;
using System.Collections.Generic;

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
    public void Add_To_Cache_Auto_CacheKey_Passing_Object_As_Field0_Adds_FieldAndPropValues_To_Key()
    {
        var input = new CacheKeyParams();

        input.Name = "TestPerson";
        input.Age = 87878;
        input.Year = DateTime.Parse("2000-12-24");

        CacheKeyParams.Phone = "9004400044";

        var result = Cache.Get(() =>
        {
            return input.Name + input.Age + input.Year + input.Flag;
        });
        var cacheKey = "common.web.cache<Add_To_Cache_Auto_CacheKey_Passing_Object_As_Field0_Adds_FieldAndPropValues_To_Key>b__0SystemLibrary.Common.Web.Tests.CacheTests+<>c__DisplayClass3_0System.StringinputNameTestPersonFlagTrueYear12/24/2000 12:00:00 AMAddressStreet 1000Age87878Phone9004400044";
        Assert.IsTrue(cacheKey.Contains("TestPerson"), "Name");
        Assert.IsTrue(cacheKey.Contains("9004400044"), "Phone");
        Assert.IsTrue(cacheKey.Contains("Street 1000"), "Address");
        Assert.IsTrue(cacheKey.Contains("2000 12:00"), "Year");
        Assert.IsTrue(cacheKey.Contains("87878"), "Age");

        var cached = Cache.Get<string>(cacheKey);

        Assert.IsTrue(cached.Is(), "Not in cache");
        Assert.IsTrue(cached.Contains("TestPerson") && cached.Contains("87878"), "Invalid text");
    }

    [TestMethod]
    public void Add_To_Cache_Auto_CacheKey_Passing_Function_As_GetItem_Success()
    {
        var getItems = () => GetText("Hello", 101, true);

        var cached = Cache.Get(GetItemsFromFunction, debug: true);
        Assert.IsTrue(cached.Is());

        cached = Cache.Get(GetItemsFromFunction);
        Assert.IsTrue(cached.Is());
        Assert.IsTrue(cached.Contains("55") && cached.Contains("World"), "Err 1: " + cached);

        var cacheKey = "common.web.cacheGetItemsFromFunctionSystemLibrary.Common.Web.Tests.CacheTestsSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cached.Contains("55") && cached.Contains("World"), "Err 2: " + cached);
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

        var cacheKey = "common.web.cache<Auto_Create_CacheKey_By_Passing_Function_Without_Outside_Vars_Success>b__6_0SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is());
        Assert.IsTrue(cachedItem.Contains("123"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success()
    {
        var cached = Cache.Get(() => GetText("Hello", 111, true));
        Assert.IsTrue(cached.Is(), "It is not  hello");

        var cacheKey = "common.web.cache<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__7_0SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Is(), "Does not exist 1");
        Assert.IsTrue(cachedItem.Contains("111"));

        var cached2 = Cache.Get(() => GetText("Hello", 222, true));
        var cacheKey2 = "common.web.cache<Auto_Create_CacheKey_By_Passing_Lambda_Without_Outside_Vars_Success>b__7_1SystemLibrary.Common.Web.Tests.CacheTests+<>cSystem.String";
        var cachedItem2 = Cache.Get<string>(cacheKey2);
        Assert.IsTrue(cachedItem2.Is(), "Does not exist 2");
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

        var cacheKey = "common.web.cache<Auto_Create_CacheKey_By_Inline_Lambda_With_Outside_Vars_Vars_Are_Part_Of_CacheKey_Success>b__0SystemLibrary.Common.Web.Tests.CacheTests+<>c__DisplayClass8_0System.StringaHellob333cTrue";
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

        var cacheKey = "common.web.cache<" + nameof(Auto_Create_CacheKey_By_Passing_Function_With_Outside_Vars_Success) + ">b__0SystemLibrary.Common.Web.Tests.CacheTests+<>c__DisplayClass9_0System.StringaHellob555cTrue";
        var cachedItem = Cache.Get<string>(cacheKey);
        Assert.IsTrue(cachedItem.Contains("555"));
    }

    [TestMethod]
    public void Auto_Create_CacheKey_By_Passing_Dictionary_Success()
    {
        var a = new Dictionary<string, string>();

        a.Add("Hello", "World");

        var getItems = () => GetText(a["Hello"].ToString());

        var cached = Cache.Get(getItems);

        var cacheKey = "common.web.cache<Auto_Create_CacheKey_By_Passing_Dictionary_Success>b__0SystemLibrary.Common.Web.Tests.CacheTests+<>c__DisplayClass10_0System.Stringa1[Hello, World]";

        var item = Cache.Get<string>(cacheKey);

        Assert.IsTrue(item != null, "Wrong cachekey");
        Assert.IsTrue(item.Contains("World99"));
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

        // Duration is part of cache key hence A is 2
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

    public class CacheKeyParams
    {
        public CacheKeyParams()
        {
            Flag = true;
            LastName = "Mr Smith";
            MiddleName = "None";
        }

        static CacheKeyParams()
        {
            Address = "Street 1000";
        }

        public string Name { get; set; }
        public bool Flag { get; }
        public int Age;
        public DateTime Year { get; set; }
        string LastName;
        string MiddleName { get; set; }

        public static string Address { get; }
        public static string Phone;
    }

    [TestMethod]
    public void TryGet_From_Cache_Throws_Returns_Default()
    {
        var item = Cache.TryGet<string>(CacheKey + "try-get-1", () => throw new Exception("OK"));

        Assert.IsTrue(item == null, "Item is not null");
    }

    [TestMethod]
    public void TryGet_From_Cache_DoesNotThrow_Returns_Default()
    {
        var item = Cache.TryGet<string>(CacheKey + "try-get-2", () => "Hello world");

        Assert.IsTrue(item == "Hello world", "Item is not null");
    }

    [TestMethod]
    public void TryGet_MultipleTimes_From_Cache_DoesNotThrow_Returns_Default()
    {
        string item;
        for (int i = 0; i < 10; i++)
        {
            item = Cache.TryGet<string>(CacheKey + "try-get-3", () => "Hello world");

            Assert.IsTrue(item == "Hello world", "Item is not null");
        }

        item = Cache.Get<string>(CacheKey + "try-get-3", () => "Hello world");
        Assert.IsTrue(item == "Hello world");

        item = Cache.Get<string>(CacheKey + "try-get-3");
        Assert.IsTrue(item == "Hello world");
    }
}
