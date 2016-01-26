using KotaeteMVC.Context;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Linq;

namespace KotaeteMVC.Service
{
    public class AnswersService : UsersService
    {
        public AnswersService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public AnswerListProfileViewModel GetAnsweredQuestionsListProfileViewModel(string userName, int page)
        {
            var query = GetAnsweredQuestionsQuery(userName);
            AnswerListProfileViewModel model = GetAnswerListProfileModelForQuery(userName, page, query);
            var initializer = new PaginationInitializer("QuestionsProfilePage", "answers-list", userName, _pageSize);
            initializer.InitializePaginationModel(model.AnswerList, page, query.Count());
            return model;
        }

        public AnswerListProfileViewModel GetAnswersListProfileViewModel(string userName, int page)
        {
            var query = GetAnswersQuery(userName);
            AnswerListProfileViewModel model = GetAnswerListProfileModelForQuery(userName, page, query);
            var initializer = new PaginationInitializer("AnswersProfilePage", "answers-list", userName, _pageSize);
            initializer.InitializePaginationModel(model.AnswerList, page, query.Count());
            return model;
        }

        public bool SaveAnswer(string content, int questionDetailId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return false;
            }
            var questionDetail = _context.QuestionDetails.FirstOrDefault(qstDetail => qstDetail.QuestionDetailId == questionDetailId);
            if (questionDetail == null || questionDetail.Answered || questionDetail.Deleted)
            {
                return false;
            }
            var user = GetCurrentUser();
            if (user == null || questionDetail.AskedTo != user)
            {
                return false;
            }
            var answer = new Answer()
            {
                Content = content,
                Deleted = false,
                QuestionDetailId = questionDetailId,
                TimeStamp = DateTime.Now,
                User = user
            };
            try
            {
                questionDetail.Answered = true;
                questionDetail.SeenByUser = true;
                _context.Answers.Add(answer);
                _context.SaveChanges();
                return true;
            }  catch (Exception e)
            {
                return false;
            }
        }

        private IQueryable<Answer> GetAnsweredQuestionsQuery(string userName)
        {
            var query = from answer in _context.Answers
                        where answer.QuestionDetail.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        answer.Deleted == false
                        orderby answer.TimeStamp descending
                        select answer;
            return query;
        }

        private AnswerListProfileViewModel GetAnswerListProfileModelForQuery(string userName, int page, IQueryable<Answer> query)
        {
            var answers = GetPageFor(query, page);
            var userProfile = GetUserProfile(userName);
            var answerModels = query.Select(answer => new AnswerProfileViewModel()
            {
                Answer = answer,
                AnswerParagraphs = answer.Content.SplitLines(),
                RepliedTimeAgo = TimeHelper.GetTimeAgo(answer.TimeStamp),
                QuestionParagraphs = answer.QuestionDetail.Question.Content.SplitLines(),
                AskerAvatarUrl = GetAvatarUrl(answer.QuestionDetail.AskedBy),
                AskedTimeAgo = TimeHelper.GetTimeAgo(answer.QuestionDetail.TimeStamp),
                ReplierAvatarUrl = GetAvatarUrl(answer.User)
            });
            var answerListModel = new AnswerListViewModel()
            {
                Answers = answerModels.ToList()
            };
            var model = new AnswerListProfileViewModel()
            {
                Profile = userProfile,
                AnswerList = answerListModel
            };
            return model;
        }

        private IQueryable<Answer> GetAnswersQuery(string userName)
        {
            var query = from answer in _context.Answers
                        where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        answer.Deleted == false
                        orderby answer.TimeStamp descending
                        select answer;
            return query;
        }
    }
}