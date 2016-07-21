using KotaeteMVC.Context;
using KotaeteMVC.Helpers;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Service
{
    public class AnswersService : UsersService
    {
        private const int CommentPageSize = 15;

        public AnswersService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public CommentViewModel CreateComment(int answerId, string comment)
        {
            var answer = _context.Answers.FirstOrDefault(ans => ans.AnswerId == answerId && ans.Active);
            if (answer == null)
            {
                return null;
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                var user = GetCurrentUser();
                var commentEntity = answer.AddComment(user, comment);
                try
                {
                    _context.SaveChanges();
                    commentEntity.AddNotifications();
                    _context.SaveChanges();
                    transaction.Commit();
                    var model = new CommentViewModel()
                    {
                        AnswerId = answerId,
                        AvatarUrl = GetAvatarUrl(user),
                        Comment = commentEntity,
                        CommentParagraphs = comment.SplitLines(),
                        ScreenName = GetUserScreenName(user.UserName),
                        UserName = user.UserName,
                        TimeAgo = TimeHelper.GetTimeAgo(commentEntity.TimeStamp),
                    };
                    return model;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
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

        public bool DeleteQuestion(int questionDetailId)
        {
            var questionDetail = _context.QuestionDetails.FirstOrDefault(qst => qst.QuestionDetailId == questionDetailId);
            if (questionDetail == null)
            {
                return false;
            }
            var currentUser = GetCurrentUser();
            if (currentUser == null || currentUser.Id != questionDetail.AskedTo.Id)
            {
                return false;
            }
            questionDetail.Active = true;
            questionDetail.SeenByUser = true;
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

        public Answer SaveAnswer(string answeringUserName, string content, int questionDetailId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var answeringUser = GetUserWithName(answeringUserName);
            if (answeringUser == null)
            {
                return null;
            }
            var questionDetail = _context.QuestionDetails.FirstOrDefault(qstDetail => qstDetail.QuestionDetailId == questionDetailId);
            if (questionDetail == null)
            {
                return null;
            }
            var answer = new Answer()
            {
                Content = content,
                Active = true,
                QuestionDetailId = questionDetailId,
                QuestionDetail = questionDetail,
                TimeStamp = DateTime.Now,
                User = answeringUser
            };
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    questionDetail.Answered = true;
                    questionDetail.SeenByUser = true;
                    _context.Answers.Add(answer);
                    _context.SaveChanges();
                    answer.AddNotification();
                    _context.SaveChanges();
                    transaction.Commit();
                    return answer;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Answer SaveAnswer(string content, int questionDetailId)
        {
            var currentUserName = GetCurrentUserName();
            return SaveAnswer(currentUserName, content, questionDetailId);
        }

        private IQueryable<Answer> GetAnsweredQuestionsQuery(string userName)
        {
            var query = from answer in _context.Answers
                        where answer.QuestionDetail.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        answer.Active
                        orderby answer.TimeStamp descending
                        select answer;
            return query;
        }

        public AnswerListProfileViewModel GetAnswerListProfileModelForQuery(string userName, int page, IQueryable<Answer> query)
        {
            var answers = GetPageFor(query, page).ToList();
            var userProfile = GetUserProfile(userName);
            var answerModels = GetAnswerModels(answers);
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

        public int GetCommentNumber(int answerId)
        {
            return _context.Comments.Count(comment => comment.Active && comment.AnswerId == answerId);
        }

        private IEnumerable<AnswerProfileViewModel> GetAnswerModels(IEnumerable<Answer> answers)
        {

            var currentUserName = GetCurrentUserName();
            return answers.Select(answer => GetAnswerModel(answer, false));
        }

        private AnswerProfileViewModel GetAnswerModel(Answer answer, bool allComments)
        {
            var likesService = new LikesService(_context, _pageSize);
            var comments = allComments ? GetCommentModels(GetCommentsFor(answer).ToList()) : ExtractFirstCommentViewModels(answer);
            return new AnswerProfileViewModel()
            {
                Answer = answer,
                AnswerParagraphs = answer.Content.SplitLines(),
                RepliedTimeAgo = TimeHelper.GetTimeAgo(answer.TimeStamp),
                QuestionParagraphs = answer.QuestionDetail.Question.Content.SplitLines(),
                AskerAvatarUrl = GetAvatarUrl(answer.QuestionDetail.AskedBy),
                AskedTimeAgo = TimeHelper.GetTimeAgo(answer.QuestionDetail.TimeStamp),
                ReplierAvatarUrl = GetAvatarUrl(answer.User),
                LikesModel = new AnswerLikeViewModel()
                {
                    HasLiked = likesService.HasLikedAnswer(GetCurrentUserName(), answer.AnswerId),
                    LikeCount = likesService.GetLikesCount(answer.AnswerId),
                    AnswerId = answer.AnswerId
                },
                Comments = comments,
                CommentsMoreButton = new MoreButtonViewModel()
                {
                    HasMore = !allComments
                }
            };
        }

        private IQueryable<Comment> GetCommentsFor(Answer answer)
        {
            return _context.Comments.Where(cmnt => cmnt.AnswerId == answer.AnswerId && cmnt.Active);
        }

        public bool HasManyCommentPages(int answerId)
        {
            return GetCommentNumber(answerId) > CommentPageSize;
        }

        private List<CommentViewModel> ExtractFirstCommentViewModels(Answer answer)
        {
            var comments = answer.Comments.Where(comment => comment.Active).OrderBy(comment => comment.TimeStamp).Take(CommentPageSize);
            return GetCommentModels(comments.ToList());
        }

        private List<CommentViewModel> GetCommentModels(List<Comment> comments)
        {
            var commentModels = comments.Select(comment => new CommentViewModel()
            {
                ScreenName = comment.User.ScreenName,
                Comment = comment,
                TimeAgo = TimeHelper.GetTimeAgo(comment.TimeStamp),
                AvatarUrl = GetAvatarUrl(comment.User),
                CommentParagraphs = comment.Content.SplitLines(),
                UserName = comment.User.UserName,
                AnswerId = comment.AnswerId
            });
            return commentModels.ToList();
        }

        private IOrderedQueryable<Answer> GetAnswersQuery(string userName)
        {
            var query = from answer in _context.Answers
                        where answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                        answer.Active
                        orderby answer.TimeStamp descending
                        select answer;
            return query;
        }

        public List<CommentViewModel> GetComments(int answerId, int page)
        {
            var commentsQuery = _context.Comments.Where(comment => comment.AnswerId == answerId && comment.Active).OrderBy(comment => comment.TimeStamp).ThenBy(comment => comment.CommentId);
            var commentList = GetPageFor(commentsQuery, page, CommentPageSize);
            return GetCommentModels(commentList.ToList());
        }

        internal AnswerProfileViewModel GetAnswerDetail(int answerId)
        {
            var answer = _context.Answers.FirstOrDefault(answ => answ.AnswerId == answerId && answ.Active);
            if (answer == null)
            {
                return null;
            }
            return GetAnswerModel(answer, true);
        }
    }
}