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
    public class AnswersController : AlertControllerBase
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
                var initializer = new PaginationInitializer<AnswerProfileViewModel>(AjaxAnswersRouteName, AnswerListId, userName, GetPageSize());
                var model = new AnswerListViewModel();
                initializer.InitializePaginationModel(model, page, query.Count());
                model.Answers = initializer.GetPage(query, page);
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
                var initializer = new PaginationInitializer<AnswerProfileViewModel>(AjaxQuestionsRouteName, AnswerListId, userName, GetPageSize());
                var model = new AnswerListViewModel();
                initializer.InitializePaginationModel(model, page, query.Count());
                model.Answers = initializer.GetPage(query, page);
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

            if (this.ExistsUserName(userName))
            {
                var profile = this.GetProfile(userName);
                var query = GetAnswersQuery(userName);
                if (query.Count() == 0)
                {
                    return View("NoAnswers", new NoAnswersProfileViewModel() { Profile = profile, IsAnswers = true });
                }
                var initializer = new PaginationInitializer<AnswerProfileViewModel>(AjaxAnswersRouteName, AnswerListId, userName, GetPageSize());
                var answerListModel = new AnswerListViewModel();
                initializer.InitializePaginationModel(answerListModel, page, query.Count());
                answerListModel.Answers = initializer.GetPage(query, page);
                if (answerListModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                var model = new AnswerListProfileViewModel()
                {
                    Profile = profile,
                    AnswerList = answerListModel
                };
                
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
                var profile = this.GetProfile(userName);
                var query = GetQuestionsQuery(userName);
                if (query.Count() == 0)
                {
                    return View("NoAnswers", new NoAnswersProfileViewModel() { Profile = profile });
                }
                var initializer = new PaginationInitializer<AnswerProfileViewModel>(AjaxQuestionsRouteName, AnswerListId, userName, GetPageSize());
                var answerListModel = new AnswerListViewModel();
                initializer.InitializePaginationModel(answerListModel, page, query.Count());
                answerListModel.Answers = initializer.GetPage(query, page);
                if (answerListModel.TotalPages < page)
                {
                    return GetPageNotFoundError();
                }
                var model = new AnswerListProfileViewModel()
                {
                    Profile = profile,
                    AnswerList = answerListModel
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
                QuestionDetail = questionDetail,
                TimeStamp = DateTime.Now,
                User = currentUser,
                QuestionDetailId = questionDetail.QuestionDetailId
            };
        }

        private IEnumerable<AnswerProfileViewModel> GetAnswerProfileViewModels(List<Answer> answersByDate)
        {
            var query = from answer in answersByDate
                        orderby answer.TimeStamp descending
                        select new AnswerProfileViewModel()
                        {
                            Answer = answer,
                            AnswerParagraphs = answer.Content.SplitLines(),
                            AskerAvatarUrl = this.GetAvatarUrl(answer.QuestionDetail.AskedBy),
                            ReplierAvatarUrl = this.GetAvatarUrl(answer.QuestionDetail.AskedTo),
                            AskedTimeAgo = this.GetTimeAgo(answer.QuestionDetail.TimeStamp),
                            QuestionParagraphs = answer.QuestionDetail.Question.Content.SplitLines(),
                            RepliedTimeAgo = this.GetTimeAgo(answer.TimeStamp)
                        };
            return query;
        }

        private IEnumerable<AnswerProfileViewModel> GetAnswersQuery(string userName)
        {
            var subQuery = from answer in Context.Answers
                           where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                           orderby answer.TimeStamp descending
                           select answer;
            return GetAnswerProfileViewModels(subQuery.ToList());
        }

        private IEnumerable<AnswerProfileViewModel> GetQuestionsQuery(string userName)
        {
            var subQuery = from answer in Context.Answers
                           where answer.QuestionDetail.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                           orderby answer.TimeStamp descending
                           select answer;
            return GetAnswerProfileViewModels(subQuery.ToList());
        }

        private ActionResult GetUserNotFoundError()
        {
            return GetErrorView(AnswerStrings.UserNotFoundErrorHeader, AnswerStrings.UserNotFoundErrorMessage);
        }
    }
}