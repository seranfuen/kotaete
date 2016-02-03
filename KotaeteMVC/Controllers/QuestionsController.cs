using KotaeteMVC.Helpers;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class QuestionsController : AlertsController
    {

        private UsersService _usersService;
        private QuestionsService _questionsService;

        public QuestionsController()
        {
            _usersService = new UsersService(Context, GetPageSize());
            _questionsService = new QuestionsService(Context, GetPageSize());
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create([Bind(Include = "AskedToUserName, QuestionContent")] QuestionDetailViewModel contentQuestion)
        {
            if (_usersService.ExistsUser(contentQuestion.AskedToUserName))
            {
                return RedirectToPrevious();
            }

            var result = false;
            if (ModelState.IsValid)
            {
                result = _questionsService.SaveQuestionDetail(contentQuestion.AskedToUserName, contentQuestion.QuestionContent);
                AddAlertSuccess(MainGlobal.QuestionAskedSuccessfullyFirstHalf + contentQuestion.AskedToScreenName + MainGlobal.QuestionAskedSuccessfullySecondHalf, "", true);
            }
            else
            {
                TempData[UserController.PreviousQuestionKey] = contentQuestion;
            }
            return RedirectToRoute("userProfile", new { @userName = contentQuestion.AskedToUserName });
        }

        [Authorize]
        [Route("AskFollowers", Name = "askFollowers")]
        public ActionResult  AskFollowers()
        {
            var model = new QuestionDetailViewModel()
            {
                AskToAllFollowers = true,
                AskedToScreenName = QuestionStrings.AllYourFollowers
            };
            return PartialView("QuestionModal", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("AskFollowers", Name = "CreateAskFollowers")]
        public ActionResult CreateAskFollowers([Bind(Include = "QuestionContent")] QuestionDetailViewModel contentQuestion)
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
                        return RedirectToPrevious();
                    }
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

    }
}