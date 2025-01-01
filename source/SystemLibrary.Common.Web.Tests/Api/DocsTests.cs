using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class DocsTests
{
    [TestMethod]
    public void Execute_Api_Controllers_Docs_Success()
    {
        var controller = new TvContentApiController();

        controller.Docs();

        var controller2 = new UserApiController();
        var res = controller2.Docs();
    }
}
