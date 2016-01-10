using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertControllerBase
    {

        [Route("user/{userName}/questions", Name = "QuestionsProfile")]
        [Route("user/{userName}/questions/{page}", Name ="QuestionsProfilePage")]
        public ActionResult ListQuestions(string userName, int page = 1)
        {
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                var query = (from answer in Context.Answers
                             where answer.Question.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                             orderby answer.TimeStamp descending
                             select answer);
                List<Answer> questionsAnsweredByDate = GetAnswersForPage(page, query);
                AnswerListProfileViewModel answerProfileViewModel = GetAnswerListProfileViewModel(userName, questionsAnsweredByDate);
                var count = query.Count();
                InitializePagination(userName, page, answerProfileViewModel, count);
                if (answerProfileViewModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                answerProfileViewModel.RouteName = "QuestionsProfilePage";
                ViewBag.Title = answerProfileViewModel.Profile.ScreenName + AnswerStrings.Questions;
                return View("AnswerList", answerProfileViewModel);
            }
            else
            {
                return GetUserNotFoundError();
            }
        }


        [Route("user/{userName}/answers/{page}", Name = "AnswersProfilePage")]
        [Route("user/{userName}/answers", Name ="AnswersProfile")]
        public ActionResult ListAnswers(string userName, int page = 1)
        {
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                var query = (from answer in Context.Answers
                             where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                             orderby answer.TimeStamp descending
                             select answer);
                var answersByDate = GetAnswersForPage(page, query);
                AnswerListProfileViewModel answerProfileViewModel = GetAnswerListProfileViewModel(userName, answersByDate);
                var count = query.Count();
                InitializePagination(userName, page, answerProfileViewModel, count);
                if (answerProfileViewModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                answerProfileViewModel.RouteName = "AnswersProfilePage";
                ViewBag.Title = answerProfileViewModel.Profile.ScreenName + AnswerStrings.Answers;
                return View("AnswerList", answerProfileViewModel);
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        private List<Answer> GetAnswersForPage(int page, IOrderedQueryable<Answer> query)
        {
            return query.Skip((page - 1) * GetPageSize()).Take(GetPageSize()).ToList();
        }

        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }

        private ActionResult GetPageNotFoundError()
        {
            return GetErrorView(AnswerStrings.PageNotFoundErrorHeader, AnswerStrings.PageNotFoundErrorMessage);
        }

        private void InitializePagination(string userName, int page, AnswerListProfileViewModel answerProfileViewModel, int count)
        {
            answerProfileViewModel.TotalPages = GetPageCount(count);
            answerProfileViewModel.CurrentPage = page;
            answerProfileViewModel.PageRouteValuesDictionary = new Dictionary<int, RouteValueDictionary>();
            for (int i = 1; i <= answerProfileViewModel.TotalPages; i++)
            {
                answerProfileViewModel.PageRouteValuesDictionary[i] = new RouteValueDictionary()
                    {
                        { "userName", userName },
                        { "page", i.ToString() }
                    };
            }
        }

        private AnswerListProfileViewModel GetAnswerListProfileViewModel(string userName, List<Answer> answersByDate)
        {
            var answerViewModel = answersByDate.Select(answer => new AnswerProfileViewModel()
            {
                Answer = answer,
                AnswerParagraphs = answer.Content.SplitLines(),
                AskerAvatarUrl = this.GetAvatarUrl(answer.Question.AskedBy),
                ReplierAvatarUrl = this.GetAvatarUrl(answer.Question.AskedTo),
                AskedTimeAgo = this.GetTimeAgo(answer.Question.TimeStamp),
                QuestionParagraphs = answer.Question.Question.Content.SplitLines(),
                RepliedTimeAgo = this.GetTimeAgo(answer.TimeStamp)
            });
            var answerProfileViewModel = new AnswerListProfileViewModel()
            {
                Answers = answerViewModel.ToList(),
                Profile = this.GetProfile(userName)
            };
            return answerProfileViewModel;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "QuestionDetailId, AnswerContent")] QuestionDetailAnswerViewModel answerViewModel)
        {
            var questionDetail = Context.QuestionDetails.FirstOrDefault(qstDetail => qstDetail.QuestionDetailId == answerViewModel.QuestionDetailId);
            var currentUser = this.GetCurrentUser();
            if (questionDetail == null)
            {
                AddAlertDanger(AnswerStrings.QuestionDetailNotFound, MainGlobal.ErrorHeader, false);
            }
            else if (currentUser.Id != questionDetail.AskedTo.Id)
            {
                AddAlertDanger(AnswerStrings.QuestionDetailNotToCurrentUser, MainGlobal.ErrorHeader, false);
            }
            else if (questionDetail.Answered)
            {
                AddAlertDanger(AnswerStrings.AlreadyAnswered, MainGlobal.ErrorHeader, true);
            } else if (questionDetail.Deleted)
            {
                AddAlertWarning(AnswerStrings.DeletedQuestionDetailWarning, "", true);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Answer answer = CreateAnswer(answerViewModel, questionDetail, currentUser);
                    try
                    {
                        questionDetail.Answered = true;
                        questionDetail.SeenByUser = true;
                        Context.Answers.Add(answer);
                        Context.SaveChanges();
                        AddAlertSuccess(AnswerStrings.SuccessAnswer, "", true);
                    }
                    catch (Exception e)
                    {
                        AddAlertDatabaseErrror(e);
                    }
                }
                else
                {
                    AddAlertDanger(ModelState.Values.First(value => value.Errors.Count() > 0).Errors.First().ErrorMessage, "", true);
                }
            }
            return RedirectToAction("Index", "Inbox");
        }

        private static Answer CreateAnswer(QuestionDetailAnswerViewModel answerViewModel, QuestionDetail questionDetail, ApplicationUser currentUser)
        {
            return new Answer()
            {
                Content = answerViewModel.AnswerContent,
                Question = questionDetail,
                TimeStamp = DateTime.Now,
                User = currentUser,
                QuestionDetailId = questionDetail.QuestionDetailId
            };
        }
    }
}