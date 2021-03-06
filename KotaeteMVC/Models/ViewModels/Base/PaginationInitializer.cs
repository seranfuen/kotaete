﻿using KotaeteMVC.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace KotaeteMVC.Models.ViewModels.Base
{
    public class PaginationInitializer
    {
        public const string PageKey = "page";
        public const string UserNameKey = "userName";

        protected int _pageSize;
        private string _route;
        private string _updateTargetId;
        private string _userName;
        private string _action;
        private string _controller;

        public PaginationInitializer(string action, string controller, string updateTargetId, string userName, int pageSize)
        {
            _action = action;
            _controller = controller;
            _updateTargetId = updateTargetId;
            _userName = userName;
            _pageSize = pageSize;
        }

        public PaginationInitializer(string route, string updateTargetId, string userName, int pageSize)
        {
            _route = route;
            _updateTargetId = updateTargetId;
            _userName = userName;
            _pageSize = pageSize;
        }

        public void InitializePaginationModel(PaginationViewModel model, int currentPage, int count)
        {
            if (_route != null)
            {
                model.Route = _route;
            }
            else
            {
                model.Controller = _controller;
                model.Action = _action;
            }
            model.CurrentPage = currentPage;
            model.TotalPages = model.GetPageCount(count, _pageSize);
            if (model.CurrentPage > model.TotalPages)
            {
                model.CurrentPage = model.TotalPages;
            }
            model.UpdateTargetId = _updateTargetId;
            InitializeRouteValueDictionary(model);
        }

        private void InitializeDictionaryForPage(PaginationViewModel model, int page)
        {
            model.PageRouteValuesDictionary[page] = new RouteValueDictionary();
            model.PageRouteValuesDictionary[page][UserNameKey] = _userName;
            model.PageRouteValuesDictionary[page][PageKey] = page.ToString();
        }

        private void InitializeRouteValueDictionary(PaginationViewModel model)
        {
            model.PageRouteValuesDictionary = new Dictionary<int, RouteValueDictionary>();
            for (int i = 1; i <= model.TotalPages; i++)
            {
                InitializeDictionaryForPage(model, i);
            }
        }
    }

    public class PaginationInitializer<TEntity> : PaginationInitializer
    {
        private PaginationCreator<TEntity> _pageCreator;

        public PaginationInitializer(string route, string updateTargetId, string userName, int pageSize) : base(route, updateTargetId, userName, pageSize)
        {
            _pageCreator = new PaginationCreator<TEntity>();
        }

        public List<TEntity> GetPage(IQueryable<TEntity> query, int page)
        {
            return _pageCreator.GetPage(query, page, _pageSize);
        }

        public List<TEntity> GetPage(IEnumerable<TEntity> query, int page)
        {
            return _pageCreator.GetPage(query, page, _pageSize);
        }
    }
}