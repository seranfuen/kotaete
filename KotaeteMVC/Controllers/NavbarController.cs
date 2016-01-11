using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class NavbarController : BaseController
    {
        // GET: Navbar
        public ActionResult Index()
        {
            var model = new NavbarViewModel()
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                InboxCount = GetInboxCount()
            };
            return PartialView("Index", model);
        }
    }
}