using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using Resources;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class InboxController : AlertsController
    {
        private InboxService _inboxService;

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
            _inboxService.UpdateQuestionsSeenByCurrentUser();
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
                ViewBag.HeaderImage = inboxViewModel.Profile.HeaderUrl;
                return View(inboxViewModel);
            }
        }

        [Authorize]
        [Route("InboxCount")]
        [HttpPost]
        public ActionResult InboxCount()
        {
            var navbarService = new NavbarService(Context);
            return Json(navbarService.GetInboxCount());
        }

        [Authorize]
        [Route("NoAnswers")]
        public ActionResult NoAnswers()
        {
            return PartialView("NoAnswers");
        }
    }
}