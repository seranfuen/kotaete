using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using KotaeteMVC.Service;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertsController
    {
        public const string AnswerListId = "answers-list";
        private const string AjaxAnswersRoute = "answersxhr";
        private const string AjaxAnswersRouteName = "AnswerListAjax";
        private const string AjaxQuestionsRoute = "questionsxhr";
        private const string AjaxQuestionsRouteName = "QuestionListAjax";
        private PaginationCreator<Answer> _paginationCreator = new PaginationCreator<Answer>();

        private AnswersService _answersService;

        public AnswersController()
        {
            _answersService = new AnswersService(Context, GetPageSize());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "QuestionDetailId, AnswerContent")] QuestionDetailAnswerViewModel answerViewModel)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = _answersService.SaveAnswer(answerViewModel.AnswerContent, answerViewModel.QuestionDetailId);
            }
            if (result)
            {
                AddAlertSuccess(AnswerStrings.SuccessAnswer, "", true);
            }
            else
            {
                AddAlertWarning(AnswerStrings.ErrorAnswer, "", true);
            }
            return RedirectToAction("Index", "Inbox");
        }

        [Route("user/{userName}/answers/{page}", Name = "AnswersProfilePage")]
        [Route("user/{userName}/answers", Name = "AnswersProfile")]
        public ActionResult ListAnswers(string userName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (_answersService.ExistsUser(userName))
            {
                var answerListProfileViewModel = _answersService.GetAnswersListProfileViewModel(userName, page);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("AnswerList", answerListProfileViewModel.AnswerList);
                }
                else
                {
                    return View("ProfileAnswerList", answerListProfileViewModel);
                }
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        [Route("user/{userName}/questions", Name = "QuestionsProfile")]
        [Route("user/{userName}/questions/{page}", Name = "QuestionsProfilePage")]
        public ActionResult ListQuestions(string userName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (_answersService.ExistsUser(userName))
            {
                var answerListProfileViewModel = _answersService.GetAnsweredQuestionsListProfileViewModel(userName, page);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("AnswerList", answerListProfileViewModel.AnswerList);
                }
                else
                {
                    return View("ProfileAnswerList", answerListProfileViewModel);
                }
            }
            else
            {
                return GetUserNotFoundError();
            }
        }


        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }
    }
}