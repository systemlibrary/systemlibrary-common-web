using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Razor;

namespace SystemLibrary.Common.Web.Extensions
{
    internal class ViewLocations : IViewLocationExpander
    {
        static string[] GetViews() => new string[]
        {
            "~/Views/{1}/{0}.cshtml",
            "~/Views/{0}.cshtml",
        };

        static string[] GetViewsForComponents() => new string[]
        {
            "~/Content/Components/{0}.cshtml",
            "~/Content/Components/{0}/Index.cshtml",
            "~/Content/Components/{1}/{0}.cshtml",
            "~/Views/Components/{0}.cshtml",
            "~/Views/Components/{1}/{0}.cshtml",
            "~/Components/{0}.cshtml",
            "~/Components/{1}/{0}.cshtml"
        };

        static string[] _AllViews;

        static string[] AllViews = (_AllViews != null) ? _AllViews :
            (_AllViews = GetViewsForComponents()
            .Concat(GetViews()).ToArray());

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
