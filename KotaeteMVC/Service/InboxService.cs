using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KotaeteMVC.Models;

namespace KotaeteMVC.Service
{
    public class InboxService : UsersService
    {
        public InboxService(KotaeteDbContext context, int pageSize) : base(context, pageSize) { }

        public InboxViewModel GetInboxViewModelCurrentUser()
        {
            
        }

        public InboxViewModel GetInboxViewModel(string userName)
        {
            var profile = GetUserProfile(userName);
            var viewModel = new InboxViewModel() { Profile = profile, QuestionDetails = GetQuestionDetailAnswerList(user) };
        }

        private List<QuestionDetailAnswerViewModel> GetQuestionDetailAnswerList(string userName)
        {
            var query = from questionDetail in _context.QuestionDetails
            where questionDetail.AskedTo.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
              && _context.Answers.Any(answer => answer.QuestionDetail == questionDetail) == false
            select questionDetail;
        }

        public void UpdateQuestionsSeenByCurrentUser()
        {
            UpdateQuestionsSeenByUser(GetCurrentUserName());
        }

        public void UpdateQuestionsSeenByUser(string userName)
        {
            var questionDetails = from questionDetail in _context.QuestionDetails
                                  where questionDetail.AskedTo.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && questionDetail.SeenByUser == false
                                  select questionDetail;
            questionDetails.ToList().ForEach(qstDetail => qstDetail.SeenByUser = true);
            _context.SaveChanges();
        }
    }
}