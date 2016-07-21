using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Service
{
    public class LikesService : UsersService
    {
        public LikesService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public AnswerLikeViewModel GetLikeButtonViewModel(int answerId)
        {
            var currentUserName = this.GetCurrentUserName();
            return new AnswerLikeViewModel()
            {
                HasLiked = HasLikedAnswer(currentUserName, answerId),
                LikeCount = GetLikesCount(answerId),
                AnswerId = answerId
            };
        }

        public int GetLikesCount(int answerId)
        {
            return _context.AnswerLikes.Count(like => like.AnswerId == answerId && like.Active);
        }

        public bool HasLikedAnswer(string userName, int answerId)
        {
            return _context.AnswerLikes.Any(like => like.Active && like.AnswerId == answerId && like.ApplicationUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public bool LikeAnswer(int answerId)
        {
            Answer answer = GetAnswerById(answerId);
            if (answer == null)
            {
                return false;
            }
            var currentUser = GetCurrentUser();
            if (currentUser == null)
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
                using (var transaction = _context.Database.BeginTransaction())
                {
                    _context.AnswerLikes.Add(answerLike);
                    try
                    {
                        _context.SaveChanges();
                        answerLike.AddNotification();
                        _context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
        }

        private Answer GetAnswerById(int answerId)
        {
            return _context.Answers.FirstOrDefault(answ => answ.AnswerId == answerId && answ.Active);
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
            var answersService = new AnswersService(_context, _pageSize);
            var user = GetUserWithName(userName);
            if (user == null)
            {
                return null;
            }
            else
            {
                var answersQuery = _context.AnswerLikes.Where(like => like.Active && like.ApplicationUserId == user.Id).OrderByDescending(like => like.TimeStamp).Select(like => like.Answer);
                var paginationInitializer = new PaginationInitializer("AnswersLikedPage", "answers-list", userName, _pageSize);
                var model = answersService.GetAnswerListProfileModelForQuery(userName, page, answersQuery);
                paginationInitializer.InitializePaginationModel(model.AnswerList, page, answersQuery.Count());
                return model;
            }
        }
    }
}