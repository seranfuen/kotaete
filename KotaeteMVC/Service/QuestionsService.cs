using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using System;
using System.Linq;

namespace KotaeteMVC.Service
{
    public class QuestionsService : UsersService
    {
        public QuestionsService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public int GetQuestionsAnsweredByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Active == false && answer.User.Id == user.Id);
        }

        public int GetQuestionsAskedByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Active == false && answer.QuestionDetail.AskedBy.Id == user.Id);
        }

        public bool SaveQuestionDetail(string askedToUserName, string questionContent)
        {
            var currentUser = GetCurrentUser();
            var askedToUser = GetUserWithName(askedToUserName);
            if (askedToUserName == null)
            {
                return false;
            }
            AddQuestionToContext(questionContent, currentUser, GetUserWithName(askedToUserName));
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void AddQuestionToContext(string questionContent, ApplicationUser currentUser, ApplicationUser askedToUser)
        {
            Question question = InitializeQuestion(questionContent, currentUser);
            AddQuestionDetailToContext(currentUser, askedToUser, question);
        }

        private void AddQuestionDetailToContext(ApplicationUser currentUser, ApplicationUser askedToUser, Question question)
        {
            var questionDetail = new QuestionDetail()
            {
                Answered = false,
                AskedBy = currentUser,
                AskedTo = askedToUser,
                Question = question,
                SeenByUser = false,
                Active = false,
                TimeStamp = question.TimeStamp
            };
            _context.QuestionDetails.Add(questionDetail);
        }

        private static Question InitializeQuestion(string questionContent, ApplicationUser currentUser)
        {
            return new Question()
            {
                AskedBy = currentUser,
                Content = questionContent,
                TimeStamp = DateTime.Now
            };
        }

        public bool AskAllFollowers(string content)
        {
            var usersService = new UsersService(_context, _pageSize);
            var followers = usersService.GetFollowers(usersService.GetCurrentUserName());
            var currentUser = usersService.GetCurrentUser();
            var question = InitializeQuestion(content, currentUser);
            foreach (var follower in followers)
            {
                AddQuestionDetailToContext(currentUser, follower, question);
            }
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}