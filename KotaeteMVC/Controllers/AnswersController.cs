using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using Resources;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertControllerBase
    {
        public const string AnswerListId = "answers-list";
        private const string AjaxAnswersRoute = "answersxhr";
        private const string AjaxAnswersRouteName = "AnswerListAjax";
        private const string AjaxQuestionsRoute = "questionsxhr";
        private const string AjaxQuestionsRouteName = "QuestionListAjax";
        private PaginationCreator<Answer> _paginationCreator = new PaginationCreator<Answer>();

        [Route("user/{userName}/" + AjaxAnswersRoute + "/{page}", Name = AjaxAnswersRouteName)]
        [HttpGet]
        public ActionResult AjaxListAnswers(string userName, int page)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return RedirectToRoute("AnswersProfilePage", new { @userName = userName, @page = page });
            }
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            if (this.ExistsUserName(userName))
            {
                var query = GetAnswersQuery(userName);
                var initializer = new PaginationInitializer<AnswerProfileViewModel, AnswerListViewModel>(AjaxAnswersRouteName, AnswerListId);
                var model = initializer.InitializeItemList(query, page, GetPageSize(), userName);
                if (page > model.TotalPages)
                {
                    return GetPageNotFoundError();
                }
                return PartialView("AnswerList", model);
            }
            else
            {
                return GetUserNotFoundError();
            }
        }

        [Route("user/{userName}/" + AjaxQuestionsRoute + "/{page}", Name = AjaxQuestionsRouteName)]
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
                var query = GetQuestionsQuery(userName);
                var initializer = new PaginationInitializer<AnswerProfileViewModel, AnswerListViewModel>(AjaxQuestionsRouteName, AnswerListId);
                var model = initializer.InitializeItemList(query, page, GetPageSize(), userName);
                if (page > model.TotalPages)
                {
                    return GetPageNotFoundError();
                }
                return PartialView("AnswerList", model);
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
                var query = GetAnswersQuery(userName);
                var initializer = new PaginationInitializer<AnswerProfileViewModel, AnswerListViewModel>(AjaxAnswersRouteName, AnswerListId);
                var listAnswersModel = initializer.InitializeItemList(query, page, GetPageSize(), userName);
                if (listAnswersModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                var model = new AnswerListProfileViewModel()
                {
                    Profile = this.GetProfile(userName),
                    AnswerList = listAnswersModel
                };
                return View("ProfileAnswerList", model);
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
                var query = GetQuestionsQuery(userName);
                var initializer = new PaginationInitializer<AnswerProfileViewModel, AnswerListViewModel>(AjaxQuestionsRouteName, AnswerListId);
                var listAnswersModel = initializer.InitializeItemList(query, page, GetPageSize(), userName);
                if (listAnswersModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                var model = new AnswerListProfileViewModel()
                {
                    Profile = this.GetProfile(userName),
                    AnswerList = listAnswersModel
                };
                return View("ProfileAnswerList", model);
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

        private IQueryable<AnswerProfileViewModel> GetAnswerProfileViewModels(IOrderedQueryable<Answer> answersByDate)
        {
            var query = from answer in answersByDate
                        orderby answer.TimeStamp descending
                        select new AnswerProfileViewModel()
                        {
                            Answer = answer,
                            AnswerParagraphs = answer.Content.SplitLines(),
                            AskerAvatarUrl = this.GetAvatarUrl(answer.Question.AskedBy),
                            ReplierAvatarUrl = this.GetAvatarUrl(answer.Question.AskedTo),
                            AskedTimeAgo = this.GetTimeAgo(answer.Question.TimeStamp),
                            QuestionParagraphs = answer.Question.Question.Content.SplitLines(),
                            RepliedTimeAgo = this.GetTimeAgo(answer.TimeStamp)
                        };
            return query;
        }

        private IQueryable<AnswerProfileViewModel> GetAnswersQuery(string userName)
        {
            var subQuery = from answer in Context.Answers
                           where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                           orderby answer.TimeStamp descending
                           select answer;
            return GetAnswerProfileViewModels(subQuery);
        }

        private IQueryable<AnswerProfileViewModel> GetQuestionsQuery(string userName)
        {
            var subQuery = from answer in Context.Answers
                           where answer.Question.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                           orderby answer.TimeStamp descending
                           select answer;
            return GetAnswerProfileViewModels(subQuery);
        }

        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }
    }
}