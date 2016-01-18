using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace KotaeteMVC.Models.ViewModels.Base
{
    public class PaginationInitializer
    {
        public const string PageKey = "page";

        private string _action;
        private string _controller;

        public PaginationInitializer(string action, string controller)
        {
            _action = action;
            _controller = controller;
            FixedRouteData = new Dictionary<string, string>();
        }

        public string UpdateTargetId { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public Dictionary<string, string> FixedRouteData { get; private set; }

        public void InitializeItemList(PaginationViewModel model)
        {
            model.Action = _action;
            model.Controller = _controller;
            model.CurrentPage = CurrentPage;
            model.TotalPages = TotalPages;
            model.UpdateTargetId = UpdateTargetId;
            InitializeRouteValueDictionary(model);
        }

        private void InitializeRouteValueDictionary(PaginationViewModel model)
        {
            model.PageRouteValuesDictionary = new Dictionary<int, RouteValueDictionary>();
            for (int i = 1; i <= TotalPages; i++)
            {
                InitializeDictionaryForPage(model, i);
            }
        }

        private void InitializeDictionaryForPage(PaginationViewModel model, int page)
        {
            model.PageRouteValuesDictionary[page] = new RouteValueDictionary();
            foreach (var pair in FixedRouteData)
            {
                model.PageRouteValuesDictionary[page][pair.Key] = pair.Value;
            }
            model.PageRouteValuesDictionary[page][PageKey] = page.ToString();
        }
    }
}