using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KotaeteMVC.Models;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models.ViewModels.Base;
using Resources;

namespace KotaeteMVC.Service
{
    public class InboxService : UsersService
    {
        public InboxService(KotaeteDbContext context, int pageSize) : base(context, pageSize) { }

        public InboxViewModel GetInboxViewModelCurrentUser(int page)
        {
            return GetInboxViewModel(GetCurrentUserName(), page);
        }

        public InboxViewModel GetInboxViewModel(string userName, int page)
        {
            var profile = GetUserProfile(userName);
            var viewModel = new InboxViewModel() { Profile = profile, QuestionDetails = GetQuestionDetailAnswerList(userName, page) };
            viewModel.ConfirmModal = new ConfirmModalViewModel("confirm-delete-modal")
            {
                Question = InboxStrings.DeleteConfirm,
                YesButton = InboxStrings.YesButton,
                NoButton = InboxStrings.NoButton
            };
            var paginationInitializer = new PaginationInitializer("InboxPage", "inbox-questions", userName, _pageSize);
            paginationInitializer.InitializePaginationModel(viewModel, page, GetIncomingQuestionsCount(userName));
            return viewModel;
        }

        private int GetIncomingQuestionsCount(string userName)
        {
            return GetIncomingQuestionsQuery(userName).Count();
        }

        private List<QuestionDetailAnswerViewModel> GetQuestionDetailAnswerList(string userName, int page)
        {
            IQueryable<QuestionDetail> query = GetIncomingQuestionsQuery(userName);
            var questionsPage = GetPageFor(query, page).ToList();

            var questions = questionsPage.Select(qst => new QuestionDetailAnswerViewModel()
            {
                QuestionDetail = qst,
                QuestionDetailId = qst.QuestionDetailId,
                AskerAvatarUrl = GetAvatarUrl(qst.AskedBy),
                AskedTimeAgo = TimeHelper.GetTimeAgo(qst.TimeStamp),
                QuestionParagraphs = qst.Question.Content.SplitLines(),
                Seen = qst.SeenByUser
            });
            return questions.ToList();
        }

        private IQueryable<QuestionDetail> GetIncomingQuestionsQuery(string userName)
        {
            return from questionDetail in _context.QuestionDetails
                   where questionDetail.Answered == false && questionDetail.Active
                   && questionDetail.AskedTo.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                   orderby questionDetail.TimeStamp descending
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