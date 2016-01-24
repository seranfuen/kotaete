using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using System.Linq;
using System;

namespace KotaeteMVC.Service
{
    public class QuestionsService : UsersService
    {
        public QuestionsService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public int GetQuestionsAnsweredByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Deleted == false && answer.User == user);
        }

        public int GetQuestionsAskedByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Deleted == false && answer.QuestionDetail.AskedBy == user);
        }

        public bool SaveQuestionDetail(string askedToUserName, string questionContent)
        {
            var currentUser = GetCurrentUser();
            var askedToUser = GetUserWithName(askedToUserName);
            if (askedToUserName == null)
            {
                return false;
            }
            AddQuestionToContext(questionContent, GetUserWithName(askedToUserName), currentUser);
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
            var question = new Question()
            {
                AskedBy = currentUser,
                Content = questionContent,
                TimeStamp = DateTime.Now
            };
            var questionDetail = new QuestionDetail()
            {
                Answered = false,
                AskedBy = currentUser,
                AskedTo = askedToUser,
                Question = question,
                SeenByUser = false,
                Deleted = false,
                TimeStamp = question.TimeStamp
            };
            _context.QuestionDetails.Add(questionDetail);
        }

        public bool AskAllFollowers(string content)
        {
            var usersService = new UsersService(_context, _pageSize);
            var followers = usersService.GetFollowers(usersService.GetCurrentUserName());
            var currentUser = usersService.GetCurrentUser();
            foreach (var follower in followers)
            {
                AddQuestionToContext(content, currentUser, follower);
            }
            try
            {
                _context.SaveChanges();
                return true;
            } catch (Exception e)
            {
                return false;
            }
        }
    }
}