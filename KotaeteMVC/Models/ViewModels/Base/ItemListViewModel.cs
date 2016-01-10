using System.Collections.Generic;
using System.Web.Routing;

namespace KotaeteMVC.Models.ViewModels.Base
{
    public abstract class ItemListViewModel
    {
        public int TotalPages { get; set; }

        public string RouteName { get; set; }

        public Dictionary<int, RouteValueDictionary> PageRouteValuesDictionary { get; set; }

        public int CurrentPage { get; set; }

    }
}