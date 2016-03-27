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

        public QuestionDetail SaveQuestionDetail(string askedUserName, string askingUserName, string question)
        {
            var askingUser = GetUserWithName(askingUserName);
            if (askingUser == null)
            {
                return null;
            }
            var askedToUser = GetUserWithName(askedUserName);
            if (askedUserName == null)
            {
                return null;
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                var questionDetail = AddQuestionToContext(question, askingUser, GetUserWithName(askedUserName));
                try
                {
                    _context.SaveChanges();
                    questionDetail.AddNotification();
                    _context.SaveChanges();
                    transaction.Commit();
                    return questionDetail;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public QuestionDetail SaveQuestionDetail(string askedUserName, string question)
        {
            return SaveQuestionDetail(askedUserName, GetCurrentUserName(), question);
        }

        private QuestionDetail AddQuestionToContext(string questionContent, ApplicationUser currentUser, ApplicationUser askedToUser)
        {
            Question question = InitializeQuestion(questionContent, currentUser);
            return AddQuestionDetailToContext(currentUser, askedToUser, question);
        }



        private QuestionDetail AddQuestionDetailToContext(ApplicationUser currentUser, ApplicationUser askedToUser, Question question)
        {
            var questionDetail = new QuestionDetail()
            {
                Answered = false,
                AskedBy = currentUser,
                AskedTo = askedToUser,
                Question = question,
                SeenByUser = false,
                Active = true,
                TimeStamp = question.TimeStamp
            };
            _context.QuestionDetails.Add(questionDetail);
            return questionDetail;
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