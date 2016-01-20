using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertControllerBase
    {
        public const string AnswerListId = "answers-list";
        private const string AjaxAnswersRoute = "answersxhr";
        private const string AjaxQuestionsRoute = "questionsxhr";

        private PaginationCreator<Answer> _paginationCreator = new PaginationCreator<Answer>();

        [Route("user/{userName}/" + AjaxAnswersRoute + "/{page}", Name = "AnswerListAjax")]
        [HttpGet]
        public ActionResult AjaxListAnswers(string userName, int page)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return RedirectToRoute("AnswersProfilePage", new { @userName = userName, @page = page });
            }
            Thread.Sleep(2500);
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                IOrderedQueryable<Answer> query = GetAnswersQuery(userName);
                List<Answer> questionsAnsweredByDate = GetAnswerListForPage(page, query);
                var answerList = GetAnswerListViewModel(questionsAnsweredByDate, page, query.Count(), userName);
                if (page > answerList.TotalPages)
                {
                    return GetPageNotFoundError();
                }
                answerList.Route = "AnswerListAjax";
                return PartialView("AnswerList", answerList);
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        [Route("user/{userName}/" + AjaxQuestionsRoute + "/{page}", Name = "QuestionListAjax")]
        [HttpGet]
        public ActionResult AjaxListQuestions(string userName, int page)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return RedirectToRoute("QuestionsProfilePage", new { @userName = userName, @page = page });
            }
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                IOrderedQueryable<Answer> query = GetQuestionsQuery(userName);
                List<Answer> questionsAnsweredByDate = GetAnswerListForPage(page, query);
                var answerModels = GetAnswerListViewModel(questionsAnsweredByDate, page, query.Count(), userName);
                if (page > answerModels.TotalPages)
                {
                    return GetPageNotFoundError();
                }
                answerModels.Route = "QuestionListAjax";
                return PartialView("AnswerList", answerModels);
            }
            else
            {
                return GetUserNotFoundError();
            }
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
            }
            else if (questionDetail.Deleted)
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

        [Route("user/{userName}/answers/{page}", Name = "AnswersProfilePage")]
        [Route("user/{userName}/answers", Name = "AnswersProfile")]
        public ActionResult ListAnswers(string userName, int page = 1)
        {
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                AnswerListProfileViewModel answerProfileViewModel = GetAnswerListProfileViewModelFor(userName, page);
                if (answerProfileViewModel.AnswerList.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                answerProfileViewModel.AnswerList.Route = "AnswerListAjax";
                ViewBag.Title = answerProfileViewModel.Profile.ScreenName + AnswerStrings.Answers;
                return View("ProfileAnswerList", answerProfileViewModel);
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
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                AnswerListProfileViewModel answerProfileViewModel = GetQuestionListProfileViewModelFor(userName, page);
                if (page > answerProfileViewModel.AnswerList.TotalPages)
                {
                    return GetPageNotFoundError();
                }
                answerProfileViewModel.AnswerList.Route = "QuestionListAjax";
                ViewBag.Title = answerProfileViewModel.Profile.ScreenName + AnswerStrings.Questions;
                return View("ProfileAnswerList", answerProfileViewModel);
            }
            else
            {
                return GetUserNotFoundError();
            }
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

        private List<Answer> GetAnswerListForPage(int page, IOrderedQueryable<Answer> query)
        {
            return _paginationCreator.GetPage(query, page, this.GetPageSize());
        }

        private AnswerListProfileViewModel GetAnswerListProfileViewModel(string userName, int page, int totalCount, List<Answer> answersByDate, string action)
        {
            IEnumerable<AnswerProfileViewModel> answerViewModel = GetAnswerProfileViewModels(answersByDate);
            var answerProfileViewModel = new AnswerListProfileViewModel(answerViewModel.ToList())
            {
                Profile = this.GetProfile(userName)
            };
            InitializePaginator(userName, page, totalCount, action, answerProfileViewModel.AnswerList);
            return answerProfileViewModel;
        }

        private AnswerListProfileViewModel GetAnswerListProfileViewModelFor(string userName, int page)
        {
            IOrderedQueryable<Answer> query = GetAnswersQuery(userName);
            List<Answer> questionsAnsweredByDate = GetAnswerListForPage(page, query);
            return GetAnswerListProfileViewModel(userName, page, query.Count(), questionsAnsweredByDate, "AjaxListAnswers");
        }

        private AnswerListViewModel GetAnswerListViewModel(List<Answer> answers, int page, int count, string userName)
        {
            var answerList = new AnswerListViewModel(GetAnswerProfileViewModels(answers));
            InitializePaginator(userName, page, count, "AjaxListAnswers", answerList);
            return answerList;
        }

        private List<AnswerProfileViewModel> GetAnswerProfileViewModels(List<Answer> answersByDate)
        {
            return answersByDate.Select(answer => new AnswerProfileViewModel()
            {
                Answer = answer,
                AnswerParagraphs = answer.Content.SplitLines(),
                AskerAvatarUrl = this.GetAvatarUrl(answer.Question.AskedBy),
                ReplierAvatarUrl = this.GetAvatarUrl(answer.Question.AskedTo),
                AskedTimeAgo = this.GetTimeAgo(answer.Question.TimeStamp),
                QuestionParagraphs = answer.Question.Question.Content.SplitLines(),
                RepliedTimeAgo = this.GetTimeAgo(answer.TimeStamp)
            }).ToList();
        }
        private IOrderedQueryable<Answer> GetAnswersQuery(string userName)
        {
            return (from answer in Context.Answers
                    where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                    orderby answer.TimeStamp descending
                    select answer);
        }

        private AnswerListProfileViewModel GetQuestionListProfileViewModelFor(string userName, int page)
        {
            IOrderedQueryable<Answer> query = GetQuestionsQuery(userName);
            List<Answer> questionsAnsweredByDate = GetAnswerListForPage(page, query);
            return GetAnswerListProfileViewModel(userName, page, query.Count(), questionsAnsweredByDate, "AjaxListQuestions");
        }

        private IOrderedQueryable<Answer> GetQuestionsQuery(string userName)
        {
            return (from answer in Context.Answers
                    where answer.Question.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                    orderby answer.TimeStamp descending
                    select answer);
        }
        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }
        private void InitializePagination(PaginationViewModel paginationModel, string userName, int page, int count, string action)
        {
            paginationModel.Action = action;
            paginationModel.Controller = "Answers";
            paginationModel.CurrentPage = page;
            paginationModel.UpdateTargetId = "answers-list";
        }

        private void InitializePaginator(string userName, int page, int totalCount, string route, PaginationViewModel model)
        {
            var initializer = new PaginationInitializer(route);
            initializer.CurrentPage = page;
            initializer.TotalPages = PaginationViewModel.GetPageCount(totalCount, GetPageSize());
            initializer.UpdateTargetId = AnswerListId;
            initializer.FixedRouteData["userName"] = userName;
            initializer.InitializeItemList(model);
        }
    }
}