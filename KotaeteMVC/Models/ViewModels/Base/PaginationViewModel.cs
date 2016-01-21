using System.Collections.Generic;
using System.Web.Routing;

namespace KotaeteMVC.Models.ViewModels.Base
{
    public static class PaginationExtension
    {
        public static int GetPageCount(int itemCount, int pageSize)
        {
            var pages = itemCount / pageSize;
            if (itemCount % pageSize > 0)
            {
                return pages + 1;
            }
            return pages;
        }
    }

    public abstract class PaginationViewModel<TEntity>
    {


        public string Route { get; set; }

        public int TotalPages { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string UpdateTargetId { get; set; }

        public Dictionary<int, RouteValueDictionary> PageRouteValuesDictionary { get; set; }

        public int CurrentPage { get; set; }

        public List<TEntity> Entities { get; set; }
    }
}