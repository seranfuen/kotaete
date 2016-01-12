using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using KotaeteMVC.Helpers;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class NavbarController : BaseController
    {
        // GET: Navbar
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                var model = new NavbarViewModel()
                {
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    InboxCount = GetInboxCount(),
                    AvatarUrl = this.GetAvatarUrl(this.GetCurrentUser()),
                    UserName = this.GetCurrentUser().ScreenName
                };
                return PartialView("Index", model);
            } else
            {
                return PartialView("Index", new NavbarViewModel());
            }
        }
    }
}