using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Web.Tests._App;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class LogWriterTests
{
    const string DumpFullPath = @"C:\Logs\systemlibrary-common-web-unit-tests.txt";

    [TestMethod]
    public void Write_Various_Log_Levels_Without_Registering_ILogWriter_Success()
    {
        System.Threading.Thread.Sleep(200);

        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Assert.IsFalse(System.IO.File.Exists(DumpFullPath));

        Log.Write("Err");

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Err"), "Err");
        Assert.IsTrue(content.Contains("Write in LogWriter:"), "Write in LogWriter:");
    }

    [TestMethod]
    public void Write_Class_Success()
    {
        App.Start<ILogWriter, LogWriter>();
        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        var car = new Car();

        car.Name = "Ferrari";
        car.Names = new string[] { "Hello1", "Hello2" };
        car.LastNames = new System.Collections.Generic.List<string> { "LastName1", "LastName2", "LastName3" };
        car.Ages = new int[] { 4, 5, 6, 7, 100 };
        car.Age = 1000;
        car.Vehicle = new Car() { Name = "Volvo" };

        car.Born = DateTime.Now;
        car.Death = DateTimeOffset.Now;
        car.IsEnabled = true;

        Log.Error(car);

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Ferrari"), "Ferrari");
    }

    [TestMethod]
    public void Write_Exception_As_Warning()
    {
        System.Threading.Thread.Sleep(100);
        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Log.Error("Custom stacktrace");

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Custom stack"));
    }

    [TestMethod]
    public void Write_Various_Log_Levels_Success()
    {
        System.Threading.Thread.Sleep(300);
        Dump.Clear();

        Log.Error("12345");

        var content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Error: "), "Error: prefix text was not added to message");
        Assert.IsTrue(content.Contains("12345"), "Text 12345 was not logged");

        Dump.Clear();

        Log.Warning("67890");

        content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Warning: "), "Warning: prefix text was not added to message");
        Assert.IsTrue(content.Contains("67890"), "Text 67890 was not logged");

        Dump.Clear();

        Log.Debug("Debugging 101");

        content = System.IO.File.ReadAllText(DumpFullPath);

        Assert.IsTrue(content.Contains("Debugging 101"), "Debugging 101");

        Dump.Clear();

        Log.Info("abcdef");

        //NOTE: appSettings.json has "Debug" assigned as log level, manually change it to Info to test this
       //content = System.IO.File.ReadAllText(DumpFullPath);
       //Assert.IsTrue(content.Contains("Info: "), "Info: prefix text was not added to message");
       //Assert.IsTrue(content.Contains("abcdef"), "Text abcdef was not logged");

        //Dump.Clear();
    }
}
