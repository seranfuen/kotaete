using KotaeteMVC.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Helpers;
using KotaeteMVC.App_GlobalResources;

namespace KotaeteMVC.Controllers
{
    public class AnswersController : AlertControllerBase
    {

        [Route("user/{userName}/answers", Name ="AnswersProfile")]
        [Route("answers/list/{userName}")]
        public ActionResult List(string userName)
        {
            if (this.ExistsUserName(userName))
            {
                var answersByDate = (from answer in Context.Answers
                                    where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && answer.Deleted == false
                                    orderby answer.TimeStamp descending
                                    select answer).ToList();
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
                return View("AnswerList", answerProfileViewModel); // create viewmodel
            } else
            {
                var errorModel = new ErrorViewModel()
                {
                    ErrorTitle = AnswerStrings.UserNotFoundErrorHeader,
                    ErrorMessage = AnswerStrings.UserNotFoundErrorMessage
                };
                return View("Error", errorModel);
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