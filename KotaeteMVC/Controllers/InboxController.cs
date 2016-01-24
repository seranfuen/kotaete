using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class InboxController : AlertControllerBase
    {
        InboxService _inboxService;

        public InboxController()
        {
            _inboxService = new InboxService(Context, GetPageSize());
        }

        [Authorize]
        public ActionResult Index()
        {
            var user = this.GetProfile(this.GetCurrentUserName());
            
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
                QuestionParagraphs = qst.Question.Content.SplitLines(),
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