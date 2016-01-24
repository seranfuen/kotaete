using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using KotaeteMVC.Helpers;
using System.Web.Mvc;
using KotaeteMVC.Service;

namespace KotaeteMVC.Controllers
{
    public class NavbarController : BaseController
    {
        private NavbarService _navbarService;

        public NavbarController()
        {
            _navbarService = new NavbarService(Context);
        }

        public ActionResult Index()
        {
            var model = _navbarService.GetNavbarViewModel(Request.IsAuthenticated);
            return PartialView("Index", model);
        }
    }
}