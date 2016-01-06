using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class QuestionsController : AlertControllerBase
    {
        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create([Bind(Include = "AskedToUserName, QuestionContent")] ContentQuestionDetailViewModel contentQuestion)
        {
            var valid = true;
            var askedUser = this.GetUserWithName(contentQuestion.AskedToUserName);
            var askingUser = this.GetCurrentUser();
            if (askingUser == null)
            {
                AddAlertDanger("The current user couldn't be authenticated", "Fatal error", false);
                valid = false;
            } else if (askedUser == null)
            {
                AddAlertDanger("There was an error retrieving the details about the user " + contentQuestion.AskedToUserName, "Fatal error", false);
            }
            if (ModelState.IsValid && valid)
            {
                Question question = InitializeQuestion(contentQuestion, askedUser);
                QuestionDetail questionDetail = NewMethod(askedUser, askingUser, question);
                try
                {
                    Context.QuestionDetails.Add(questionDetail);
                    Context.SaveChanges();
                }
                catch (Exception e)
                {
                    AddAlertDatabaseErrror(e);
                }
                AddAlertSuccess(MainGlobal.QuestionAskedSuccessfullyFirstHalf + askedUser.ScreenName + MainGlobal.QuestionAskedSuccessfullySecondHalf, "", true);
            }
            else
            {
                TempData[UserController.PreviousQuestionKey] = contentQuestion;
            }
            return RedirectToRoute("userProfile", new { @userName = contentQuestion.AskedToUserName });
        }

        private static QuestionDetail NewMethod(ApplicationUser askedUser, ApplicationUser askingUser, Question question)
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

        private static Question InitializeQuestion(ContentQuestionDetailViewModel contentQuestion, ApplicationUser askedUser)
        {
            return new Question()
            {
                AskedBy = askedUser,
                Content = contentQuestion.QuestionContent,
                TimeStamp = DateTime.Now
            };
        }
    }
}