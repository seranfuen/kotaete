using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Service
{
    public class LikesService : AnswersService
    {
        public LikesService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public bool LikeAnswer(int answerId)
        {
            Answer answer = GetAnswerById(answerId);
            if (answer == null)
            {
                return false;
            }
            var currentUser = GetCurrentUser();
            if (answer.User.Id == currentUser.Id)
            {
                return false;
            }
            if (_context.AnswerLikes.Any(like => like.ApplicationUserId == currentUser.Id && like.AnswerId == answer.AnswerId && like.Active))
            {
                return false;
            }
            else
            {
                var answerLike = new AnswerLike()
                {
                    Active = true,
                    Answer = answer,
                    ApplicationUser = currentUser,
                    TimeStamp = DateTime.Now
                };
                _context.AnswerLikes.Add(answerLike);
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

        private Answer GetAnswerById(int answerId)
        {
            return _context.Answers.FirstOrDefault(answ => answ.AnswerId == answerId && answ.Deleted == false);
        }

        public bool UnlikeAnswer(int answerId)
        {
            var answer = GetAnswerById(answerId);
            if (answer == null)
            {
                return false;
            }
            var currentUser = GetCurrentUser();
            var answerLike = _context.AnswerLikes.FirstOrDefault(like => like.Active && like.ApplicationUserId == currentUser.Id &&
                        like.AnswerId == answerId);
            if (answerLike == null)
            {
                return false;
            }
            else
            {
                answerLike.Active = false;
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

        public AnswerListProfileViewModel GetLikedAnswerListProfileModel(string userName, int page)
        {
            var user = GetUserWithName(userName);
            if (user == null)
            {
                return null;
            }
            else
            {
                var answersQuery = _context.AnswerLikes.Where(like => like.Active && like.ApplicationUserId == user.Id).Select(like => like.Answer);
                return GetAnswerListProfileModelForQuery(userName, page, answersQuery);
            }
        }
    }
}