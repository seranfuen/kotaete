using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using KotaeteMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class InboxController : AlertsController
    {
        InboxService _inboxService;

        public InboxController()
        {
            _inboxService = new InboxService(Context, GetPageSize());
        }

        [Route("Inbox", Name = "Inbox")]
        [Route("Inbox/{page}", Name = "InboxPage")]
        [Authorize]
        public ActionResult Index(int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            var inboxViewModel = _inboxService.GetInboxViewModelCurrentUser(page);
            if (page > inboxViewModel.TotalPages)
            {
                page = inboxViewModel.TotalPages;
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView(inboxViewModel.QuestionDetails);
            }
            else
            {
                return View(inboxViewModel);
            }
        }
    }
}