using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace SystemLibrary.Common.Web;

/// <summary>
/// HtmlHelperFactory lets you get a new HtmlBuilder in the backend, outside of your View Context
/// - Either use injection or simply call its ctor
/// </summary>
public class HtmlHelperFactory
{
    //Creds to: https://stackoverflow.com/questions/42039269/create-custom-html-helper-in-asp-net-core/51466436#51466436

    /// <summary>
    /// Returns a built generic HtmlHelper
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var factory = new HtmlHelperFactory();
    /// var htmlHelper = factory.Build&lt;Car&gt;();
    /// </code>
    /// </example>
    public IHtmlHelper<T> Build<T>() where T : class
    {
        var viewContext = GetViewContext();

        var htmlHelper = Services.Get<IHtmlHelper<T>>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);
        return htmlHelper;
    }

    /// <summary>
    /// Returns a built HtmlHelper
    /// </summary>
    /// <example>
    /// Usage:
    /// <code class="language-csharp hljs">
    /// var factory = new HtmlHelperFactory();
    /// var htmlHelper = factory.Build();
    /// </code>
    /// </example>
    public IHtmlHelper Build()
    {
        var viewContext = GetViewContext();

        var htmlHelper = Services.Get<IHtmlHelper>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);
        return htmlHelper;
    }

    static ViewContext GetViewContext()
    {
        var modelMetadataProvider = Services.Get<IModelMetadataProvider>();

        var tempDataProvider = Services.Get<ITempDataProvider>();

        return new ViewContext(
            new ActionContext(HttpContextInstance.Current, HttpContextInstance.Current.GetRouteData(), new ControllerActionDescriptor()),
            new DummyView(),
            new ViewDataDictionary(modelMetadataProvider, new ModelStateDictionary()),
            new TempDataDictionary(HttpContextInstance.Current, tempDataProvider),
            TextWriter.Null,
            new HtmlHelperOptions()
        );
    }

    internal class DummyView : IView
    {
        public Task RenderAsync(ViewContext context)
        {
            return Task.CompletedTask;
        }

        public string Path => "Index";
    }
}