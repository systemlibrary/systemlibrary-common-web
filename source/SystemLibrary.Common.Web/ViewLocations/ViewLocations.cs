using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Web.Extensions
{
    internal class ViewLocations : IViewLocationExpander
    {
        static string[] GetViewsForComponents() => new string[]
        {
            "~/Views/Components/{0}/Index.cshtml",
            "~/Views/Components/{0}/{0}.cshtml",
            "~/Views/Components/{1}/{0}.cshtml",
            "~/Views/Components/{0}.cshtml"
        };

        static string[] _AllViews;

        static string[] AllViews = (_AllViews != null) ? _AllViews :
            (_AllViews = GetViewsForComponents().ToArray());

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (viewLocations != null)
                return viewLocations.Concat(AllViews);

            return AllViews;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
