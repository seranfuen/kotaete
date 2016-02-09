using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;

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
                    AddAlertSuccess(string.Format(AnswerStrings.SuccessAnswer, answerViewModel.AskerScreenName), "", true);
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
        [MultipleButton(Name = "action", Argument = "Delete")]
        public ActionResult DeleteQuestion([Bind(Include = "QuestionDetailId")] QuestionDetailAnswerViewModel answerViewModel)
        {
            var result = _answersService.DeleteQuestion(answerViewModel.QuestionDetailId);
            if (result)
            {
                if (Request.IsAjaxRequest())
                {
                    return Json("deleted");
                }
                else
                {
                    AddAlertSuccess(AnswerStrings.DeletedSuccess, "", true);
                    return RedirectToPrevious();
                }
            } else
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
                    ViewBag.Title = answerListProfileViewModel.Profile.ScreenName + AnswerStrings.Answers;
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
                    if (answerListProfileViewModel.AnswerList.Answers.Any() == false)
                    {
                        ViewBag.NoAnswersMessage = AnswerStrings.NoQuestions;
                        ViewBag.Title = userName + " - " + AnswerStrings.NoQuestionsTitle;
                        return View("NoAnswers", answerListProfileViewModel.Profile);
                    }
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
                    AddAlertSuccess(AnswerStrings.CommentPostSuccess, "", true);
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
    }
}