using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace KotaeteMVC.Helpers.RazorHelper
{
    public static class HelperExtensions
    {
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, null);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }

        public static MvcHtmlString RawRouteLink(this AjaxHelper ajaxHelper, string linkText, string routeName, RouteValueDictionary routeValues, AjaxOptions ajaxOptions)
        {
            var repID = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.RouteLink(repID, routeName, routeValues, ajaxOptions, null);
            return MvcHtmlString.Create(lnk.ToString().Replace(repID, linkText));
        }
    }
}