﻿using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using System;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertsController
    {
        private PaginationCreator<Answer> _paginationCreator = new PaginationCreator<Answer>();

        private AnswersService _answersService;

        public AnswersController()
        {
            _answersService = new AnswersService(Context, GetPageSize());
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "Answer")]
        public ActionResult Create([Bind(Include = "QuestionDetailId, AnswerContent, AskerScreenName")] QuestionDetailAnswerViewModel answerViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _answersService.SaveAnswer(answerViewModel.AnswerContent, answerViewModel.QuestionDetailId);
                if (Request.IsAjaxRequest())
                {
                    return Json("answer");
                }
                else
                {
                    AddAlertSuccess(string.Format(AnswerStrings.SuccessAnswer, answerViewModel.AskerScreenName), "");
                    return RedirectToPrevious();
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return GetBadRequestResult();
                }
                else
                {
                    AddAlertDanger(AnswerStrings.EmptyAnswerError);
                    return RedirectToPrevious();
                }
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult DeleteQuestionAjax(int questionDetailId)
        {
            var result = _answersService.DeleteQuestion(questionDetailId);
            if (result)
            {
                return Json("deleted");
            }
            else
            {
                return GetBadRequestResult();
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteQuestion([Bind(Include = "QuestionDetailId")] QuestionDetailAnswerViewModel answerViewModel)
        {
            var result = _answersService.DeleteQuestion(answerViewModel.QuestionDetailId);
            if (result)
            {
                AddAlertSuccess(AnswerStrings.DeletedSuccess, "");
                return RedirectToPrevious();
            }
            else
            {
                return GetBadRequestResult();
            }
        }

        [Route("user/{userName}/answers/liked/{page}", Name = "AnswersLikedPage")]
        [Route("user/{userName}/answers/liked", Name = "AnswersLiked")]
        public ActionResult ListLikedAnswers(string userName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            var likesService = new LikesService(Context, GetPageSize());
            if (_answersService.ExistsUser(userName))
            {
                var answers = likesService.GetLikedAnswerListProfileModel(userName, page);
                InitializeMoreButton(answers.AnswerList);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("AnswerList", answers.AnswerList);
                }
                else
                {
                    if (answers.AnswerList.Answers.Any() == false)
                    {
                        ViewBag.NoAnswersMessage = AnswerStrings.NoLikes;
                        ViewBag.Title = userName + " - " + AnswerStrings.NoLikesTitle;
                        return View("NoAnswers", answers.Profile);
                    }
                    ViewBag.Title = answers.Profile.ScreenName + AnswerStrings.Likes;
                    ViewBag.HeaderImage = answers.Profile.HeaderUrl;
                    return View("ProfileAnswerList", answers);
                }
            }
            else
            {
                return GetUserNotFoundError();
            }
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
                InitializeMoreButton(answerListProfileViewModel.AnswerList);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("AnswerList", answerListProfileViewModel.AnswerList);
                }
                else
                {
                    if (answerListProfileViewModel.AnswerList.Answers.Any() == false)
                    {
                        ViewBag.NoAnswersMessage = AnswerStrings.NoAnswers;
                        ViewBag.Title = userName + " - " + AnswerStrings.NoAnswersTitle;
                        return View("NoAnswers", answerListProfileViewModel.Profile);
                    }
                    ViewBag.HeaderImage = answerListProfileViewModel.Profile.HeaderUrl;
                    ViewBag.Title = answerListProfileViewModel.Profile.ScreenName + AnswerStrings.Answers;
                    return View("ProfileAnswerList", answerListProfileViewModel);
                }
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        private void InitializeMoreButton(AnswerListViewModel answerList)
        {
            foreach (var answer in answerList.Answers)
            {
                var button = answer.CommentsMoreButton;
                button.RequestUrl = Url.RouteUrl("Comments", new { answerId = answer.Answer.AnswerId, page = 2 });
                button.HasMore = _answersService.HasManyCommentPages(answer.Answer.AnswerId);
                button.TargetElementId = "comments-" + answer.Answer.AnswerId;
                answer.TotalComments = _answersService.GetCommentNumber(answer.Answer.AnswerId);
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
                InitializeMoreButton(answerListProfileViewModel.AnswerList);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("AnswerList", answerListProfileViewModel.AnswerList);
                }
                else
                {
                    if (answerListProfileViewModel.AnswerList.Answers.Any() == false)
                    {
                        ViewBag.NoAnswersMessage = AnswerStrings.NoQuestions;
                        ViewBag.Title = userName + " - " + AnswerStrings.NoQuestionsTitle;
                        return View("NoAnswers", answerListProfileViewModel.Profile);
                    }
                    ViewBag.HeaderImage = answerListProfileViewModel.Profile.HeaderUrl;
                    ViewBag.Title = answerListProfileViewModel.Profile.ScreenName + AnswerStrings.Questions;
                    return View("ProfileAnswerList", answerListProfileViewModel);
                }
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "AnswerId, Content")] CommentViewModel commentViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _answersService.CreateComment(commentViewModel.AnswerId, commentViewModel.Content);
                if (result == null)
                {
                    return GetBadRequestResult();
                }
                if (Request.IsAjaxRequest())
                {
                    return PartialView("CommentDetail", result);
                }
                else
                {
                    AddAlertSuccess(AnswerStrings.CommentPostSuccess, "");
                    return RedirectToPrevious();
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return GetBadRequestResult();
                }
                else
                {
                    AddAlertDanger(AnswerStrings.CommentRequired);
                    return RedirectToPrevious();
                }
            }
        }

        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }

        public ActionResult PostCommentForm(AnswerProfileViewModel model)
        {
            var comment = new CommentViewModel()
            {
                AnswerId = model.Answer.AnswerId
            };
            return PartialView("CommentPost", comment);
        }

        [Route("answer/{answerId}", Name = "Answer")]
        [HttpGet]
        public ActionResult AnswerDetail(int answerId)
        {
            if (Request.IsAjaxRequest())
            {
                return GetBadRequestResult();
            }
            else
            {
                var answerModel = _answersService.GetAnswerDetail(answerId);
                if (answerModel == null)
                {
                    return GetBadRequestResult();
                }
                else
                {
                    answerModel.ShowingFullDetail = true;
                    return View("AnswerProfileDetail", answerModel);
                }
            }
        }

        [Route("answer/{answerId}/comments/{page}", Name = "Comments")]
        [HttpPost]
        public ActionResult Comments(int answerId, int page)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return GetBadRequestResult();
            }
            else
            {
                var comments = _answersService.GetComments(answerId, page);
                if (comments == null)
                {
                    return GetBadRequestResult();
                }
                var stringWriter = new StringWriter();
                var view = ViewEngines.Engines.FindPartialView(ControllerContext, "CommentList");
                ViewData.Model = comments;
                var viewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, stringWriter);
                viewContext.View.Render(viewContext, stringWriter);
                return Json(new
                {
                    html = stringWriter.ToString(),
                    url = Url.RouteUrl("Comments", new { answerId = answerId, page = page + 1 }),
                    hasMore = comments.Any()
                });
            }
        }
    }
}