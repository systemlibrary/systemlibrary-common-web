using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace SystemLibrary.Common.Web;

public class HtmlHelperFactory
{
    internal class DummyView : IView
    {
        public Task RenderAsync(ViewContext context)
        {
            return Task.CompletedTask;
        }

        public string Path => "Index";
    }

    //Creds to: https://stackoverflow.com/questions/42039269/create-custom-html-helper-in-asp-net-core/51466436#51466436
    public IHtmlHelper<T> Build<T>() where T : class
    {
        var viewContext = GetViewContext();

        var htmlHelper = HttpContextInstance.Current.RequestServices.GetRequiredService<IHtmlHelper<T>>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);
        return htmlHelper;
    }

    public IHtmlHelper Build()
    {
        var viewContext = GetViewContext();

        var htmlHelper = HttpContextInstance.Current.RequestServices.GetRequiredService<IHtmlHelper>();

        ((IViewContextAware)htmlHelper).Contextualize(viewContext);
        return htmlHelper;
    }

    static ViewContext GetViewContext()
    {
        var modelMetadataProvider = HttpContextInstance.Current.RequestServices.GetRequiredService<IModelMetadataProvider>();

        var tempDataProvider = HttpContextInstance.Current.RequestServices.GetRequiredService<ITempDataProvider>();

        return new ViewContext(
            new ActionContext(HttpContextInstance.Current, HttpContextInstance.Current.GetRouteData(), new ControllerActionDescriptor()),
            new DummyView(),
            new ViewDataDictionary(modelMetadataProvider, new ModelStateDictionary()),
            new TempDataDictionary(HttpContextInstance.Current, tempDataProvider),
            TextWriter.Null,
            new HtmlHelperOptions()
        );
    }
}