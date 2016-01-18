using KotaeteMVC.Helpers;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class QuestionsController : AlertControllerBase
    {
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create([Bind(Include = "AskedToUserName, QuestionContent, AskToAllFollowers")] QuestionDetailViewModel contentQuestion)
        {
            var askingUser = this.GetCurrentUser();
            Question question = InitializeQuestion(contentQuestion, askingUser);
            List<QuestionDetail> questionDetails = null;
            if (contentQuestion.AskToAllFollowers == false)
            {
                if (ModelState.IsValid)
                {
                    var askedUser = this.GetUserWithName(contentQuestion.AskedToUserName);
                    if (askedUser == null)
                    {
                        return this.GetErrorView(QuestionStrings.UserNotFoundHeader, QuestionStrings.UserNotFoundMessage);
                    }

                    questionDetails = CreateQuestionDetails(question, askedUser);
                    AddAlertSuccess(MainGlobal.QuestionAskedSuccessfullyFirstHalf + askedUser.ScreenName + MainGlobal.QuestionAskedSuccessfullySecondHalf, "", true);
                }
                else
                {
                    TempData[UserController.PreviousQuestionKey] = contentQuestion;
                }
                return RedirectToRoute("userProfile", new { @userName = contentQuestion.AskedToUserName });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (askingUser.Followers.Any() == false)
                    {
                        return GetErrorView(QuestionStrings.NoFollowersHeader, QuestionStrings.NoFollowersMessage);
                    }
                    questionDetails = CreateQuestionDetails(question, askingUser.Followers.ToArray());
                    Context.QuestionDetails.AddRange(questionDetails);
                    Context.SaveChanges();
                    return RedirectToRoute("userProfile", new { @userName = askingUser.UserName });
                }
                else
                {
                    TempData[UserController.PreviousQuestionKey] = contentQuestion;
                    return RedirectToRoute("askFollowers");
                }
            }
        }

        private Question InitializeQuestion(QuestionDetailViewModel contentQuestion, ApplicationUser askingUser)
        {
            return new Question()
            {
                AskedBy = askingUser,
                Content = contentQuestion.QuestionContent,
                TimeStamp = DateTime.Now
            };
        }

        private List<QuestionDetail> CreateQuestionDetails(Question question, params ApplicationUser[] askedUsers)
        {
            return askedUsers.Select(user => new QuestionDetail()
            {
                Deleted = false,
                Answered = false,
                AskedBy = question.AskedBy,
                AskedTo = user,
                Question = question,
                SeenByUser = false,
                TimeStamp = DateTime.Now
            }).ToList();
        }
        private static QuestionDetail InitializeQuestionDetail(ApplicationUser askedUser, ApplicationUser askingUser, Question question)
        {
            return new QuestionDetail()
            {
                Answered = false,
                TimeStamp = question.TimeStamp,
                AskedBy = askingUser,
                AskedTo = askedUser,
                Question = question,
                Deleted = false,
                SeenByUser = false
            };
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

    }
}