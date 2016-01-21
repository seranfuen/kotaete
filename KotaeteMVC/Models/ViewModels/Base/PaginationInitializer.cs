using KotaeteMVC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace KotaeteMVC.Models.ViewModels.Base
{
    public class PaginationInitializer<TEntity, TViewModel> where TViewModel : PaginationViewModel<TEntity>, new()
    {
        public const string PageKey = "page";

        private string _route;
        private PaginationCreator<TEntity> _paginationCreator = new PaginationCreator<TEntity>();

        public PaginationInitializer(string route, string updateTargetId)
        {
            _route = route;
            UpdateTargetId = updateTargetId;
            FixedRouteData = new Dictionary<string, string>();
        }

        public string UpdateTargetId { get; set; }

        public int TotalPages { get; private set; }

        public int CurrentPage { get; private set; }

        public Dictionary<string, string> FixedRouteData { get; private set; }

        public TViewModel InitializeItemList(IQueryable<TEntity> entitiesQuery, int page, int pageSize, string userName)
        {
            TViewModel model = new TViewModel();
            CurrentPage = page;
            TotalPages = PaginationExtension.GetPageCount(entitiesQuery.Count(), pageSize);
            InitializeItemList(model);
            model.Entities = _paginationCreator.GetPage(entitiesQuery, page, pageSize).ToList();
            return model;
        }

        public void InitializeItemList(PaginationViewModel<TEntity> model)
        {
            model.Route = _route;
            model.CurrentPage = CurrentPage;
            model.TotalPages = TotalPages;
            model.UpdateTargetId = UpdateTargetId;
            InitializeRouteValueDictionary(model);
        }

        private void InitializeRouteValueDictionary(PaginationViewModel<TEntity> model)
        {
            model.PageRouteValuesDictionary = new Dictionary<int, RouteValueDictionary>();
            for (int i = 1; i <= TotalPages; i++)
            {
                InitializeDictionaryForPage(model, i);
            }
        }

        private void InitializeDictionaryForPage(PaginationViewModel<TEntity> model, int page)
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