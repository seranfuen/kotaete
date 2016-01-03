using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
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
        public ActionResult Create([Bind(Include = "ScreenName, QuestionContent")] ProfileQuestionViewModel question)
        {
            if (string.IsNullOrWhiteSpace(question.ScreenName))
            {
                AddAlertDanger("No user to ask question specified!");
                if (Request.UrlReferrer != null)
                {
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            var askedUser = this.GetUserWithName(question.ScreenName);
            ApplicationUser asker = this.GetCurrentUser();
            var now = DateTime.Now;
            var askedUserProfile = this.GetProfileQuestionViewModel(question.ScreenName);
            askedUserProfile.QuestionContent = question.QuestionContent;
            var qstDetail = new QuestionDetail()
            {
                Question = new Question()
                {
                    Content = question.QuestionContent,
                    TimeStamp = now,
                    AskedBy = asker,
                },
                AskedTo = askedUser,
                AskedBy = asker,
                TimeStamp = now,
                SeenByUser = false
            };
            var result = TryValidateModel(qstDetail.Question);
            if (result)
            {
                Context.QuestionDetails.Add(qstDetail);
                Context.SaveChanges();
                AddAlertSuccess("Question asked", "", true);
            }
            return RedirectToRoute("userProfile", new { @userName = question.ScreenName });
        }
    }
}