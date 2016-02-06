using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using Resources;
using System.Net;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class QuestionsController : AlertsController
    {
        private QuestionsService _questionsService;
        private UsersService _usersService;

        public QuestionsController()
        {
            _usersService = new UsersService(Context, GetPageSize());
            _questionsService = new QuestionsService(Context, GetPageSize());
        }

        [Authorize]
        [Route("AskFollowers", Name = "askFollowers")]
        public ActionResult AskFollowers()
        {
            var model = new QuestionDetailViewModel();
            if (Request.IsAjaxRequest())
            {
                ViewBag.AjaxEnabled = true;
                return PartialView("QuestionModal", model);
            }
            else
            {
                ViewBag.AjaxEnabled = false;
                return PartialView("QuestionAll", model);
            }
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create([Bind(Include = "AskedToUserName, QuestionContent")] QuestionDetailViewModel contentQuestion)
        {
            if (ModelState.IsValid)
            {
                var result = _questionsService.SaveQuestionDetail(contentQuestion.AskedToUserName, contentQuestion.QuestionContent);
                if (result)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        AddAlertSuccess(string.Format(QuestionStrings.QuestionAskedSuccess, contentQuestion.AskedToScreenName), "", true);
                        return RedirectToPrevious();
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                AddAlertDanger(QuestionStrings.QuestionContentMissing, "", false);
                return RedirectToPrevious();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AskFollowers", Name = "CreateAskFollowers")]
        public ActionResult CreateQuestionAllFollowers([Bind(Include = "QuestionContent")] QuestionDetailViewModel contentQuestion)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = _questionsService.AskAllFollowers(contentQuestion.QuestionContent);
                if (result)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Accepted);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            else if (Request.IsAjaxRequest() == false)
            {
                ViewBag.AjaxEnabled = false;
                return View("QuestionAll", contentQuestion);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}