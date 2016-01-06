using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class InboxController : AlertControllerBase
    {

        [Authorize]
        public ActionResult Index()
        {
            var user = this.GetProfile(this.GetCurrentUserName());
            var viewModel = new ProfileQuestionDetailViewModel() { Profile = user, QuestionDetails = GetQuestionDetailAnswerList(user) };
            Context.SaveChanges();
            return View(viewModel);
        }

        private List<QuestionDetailAnswerViewModel> GetQuestionDetailAnswerList(ProfileViewModel profileModel)
        {
            var user = profileModel.User;
            var questions = user.QuestionsReceived.Select(qst => new QuestionDetailAnswerViewModel()
            {
                QuestionDetail = qst,
                QuestionDetailId = qst.QuestionDetailId,
                AskerAvatarUrl = this.GetAvatarUrl(qst.AskedBy),
                AskedTimeAgo = this.GetTimeAgo(qst.TimeStamp),
                QuestionParagraphs = qst.Question.Content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList(),
                Seen = qst.SeenByUser
            }).Where(qst => qst.QuestionDetail.Answered == false).OrderByDescending(qst => qst.QuestionDetail.TimeStamp).ToList();
            foreach (var question in user.QuestionsReceived)
            {
                question.SeenByUser = true;
            }
            return questions;
        }
    }
}