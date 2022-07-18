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
        if (System.IO.File.Exists(DumpFullPath))
            System.IO.File.Delete(DumpFullPath);

        Assert.IsFalse(System.IO.File.Exists(DumpFullPath));

        Log.Write("Err");

        Assert.IsTrue(System.IO.File.Exists(DumpFullPath));
    }

    [TestMethod]
    public void Write_Various_Log_Levels_Success()
    {
        App.Start<ILogWriter, LogWriter>();

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

        Log.Info("abcdef");

        //NOTE: appSettings.json has "Warning" assigned as log level, manually change it to Info to test this
        //content = System.IO.File.ReadAllText(DumpFullPath);
        //Assert.IsTrue(content.Contains("Info: "), "Info: prefix text was not added to message");
        //Assert.IsTrue(content.Contains("abcdef"), "Text abcdef was not logged");

        Dump.Clear();
    }
}
