using System;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.NotificationModels;
using KotaeteMVC.Service;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Controllers
{
    public class NotificationsController : AlertsController
    {
        private NotificationsService _notificationsService;

        public NotificationsController()
        {
            _notificationsService = new NotificationsService(Context);
        }

        public ActionResult Index()
        {
            var currentUser = _notificationsService.GetCurrentUserName();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var listNotifications = GetLastNotificationPartialViews(currentUser, 20);
            throw new NotImplementedException(); // Create model that includes current user profile to display in the profile layout
            //return View("Notifications", listNotifications); 
        }

        private List<ActionResult> GetLastNotificationPartialViews(string userName, int count)
        {
            var notifications = _notificationsService.GetLastUserNotifications(userName, count);
            return null;
        }

        private ActionResult GetNotificationPartialView(IEventEntity eventEntity)
        {
            if (eventEntity is Comment)
            {
                return GetCommentNotificationPartialView(eventEntity as Comment);
            }
            else if (eventEntity is Answer)
            {
                return GetAnswerNotificationPartialView(eventEntity as Answer);
            }
            else if (eventEntity is AnswerLike)
            {
                return GetAnswerLikedNotificationPartialView(eventEntity as AnswerLike);
            }
            else if (eventEntity is QuestionDetail)
            {
                return GetQuestionAskedNotificationPartialView(eventEntity as QuestionDetail);
            }
            else if (eventEntity is Relationship)
            {
                return GetRelationshipNotificationPartialView(eventEntity as Relationship);
            }
            else
            {
                return null;
            }
        }

        private ActionResult GetCommentNotificationPartialView(Comment comment)
        {
            var model = GetCommentNotificationModel(comment);
            return PartialView("CommentNotification", model);
        }

        private ActionResult GetAnswerNotificationPartialView(Answer answer)
        {
            var model = GetAnsweredNotificationModel(answer);
            return PartialView("AnswerNotification", model);
        }

        private ActionResult GetAnswerLikedNotificationPartialView(AnswerLike answerLLike)
        {
            var model = GetAnswerLikeNotificationModel(answerLLike);
            return PartialView("AnswerLikeNotification", model);
        }

        private ActionResult GetQuestionAskedNotificationPartialView(QuestionDetail questionDetail)
        {
            var model = GetQuestionAskedNotificationModel(questionDetail);
            return PartialView("QuestionNotification", model);
        }

        private ActionResult GetRelationshipNotificationPartialView(Relationship relationship)
        {
            var model = GetRelationshipNotificationModel(relationship);
            return PartialView("RelationshipNotification", model);
        }

        private QuestionAskedNotificationViewModel GetQuestionAskedNotificationModel(QuestionDetail questionDetail)
        {
            var model = new QuestionAskedNotificationViewModel()
            {
                AskedUser = GetProfileFor(questionDetail.AskedTo),
                AskingUser = GetProfileFor(questionDetail.AskedBy),
                TimeStamp = questionDetail.TimeStamp,
                QuestionDetailId = questionDetail.QuestionDetailId
            };
            return model;
        }

        private CommentNotificationViewModel GetCommentNotificationModel(Comment comment)
        {
            var type = GetCommentNotificationType(comment);
            var model = new CommentNotificationViewModel()
            {
                AnswerId = comment.AnswerId,
                AnsweringUser = GetProfileFor(comment.Answer.User),
                CommentingUser = GetProfileFor(comment.User),
                CommentNotificationType = type,
                TimeStamp = comment.TimeStamp
            };
            return model;
        }

        private CommentNotificationViewModel.CommentNotificationTypeEnum GetCommentNotificationType(Comment comment)
        {
            if (IsCurrentUser(comment.Answer.User) && !IsCurrentUser(comment.User))
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CurrentUserAnswer;
            }
            if (comment.User == comment.Answer.User)
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CommentingUserIsAnsweringUser;
            } else
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CurrentUserCommentedAnswer;
            }
        }

        private bool IsCurrentUser(ApplicationUser user)
        {
            return IsCurrentUser(user.UserName);
        }

        private AnswerLikeNotificationViewModel GetAnswerLikeNotificationModel(AnswerLike like)
        {
            var type = IsCurrentUser(like.Answer.User.UserName) ? AnswerLikeNotificationViewModel.AnswerLikeNotificationTypeEnum.CurrentUserAnswer :
                AnswerLikeNotificationViewModel.AnswerLikeNotificationTypeEnum.OtherUsers;

            var model = new AnswerLikeNotificationViewModel()
            {
                AnswerId = like.AnswerId,
                AnswerLikeNotificationType = type,
                AnswerUser = GetProfileFor(like.Answer.User),
                LikingUser = GetProfileFor(like.ApplicationUser),
                TimeStamp = like.TimeStamp
            };
            return model;
        }

        private bool IsCurrentUser(string userName)
        {
            var currentUser = _notificationsService.GetCurrentUserName();
            return userName.Equals(currentUser, System.StringComparison.InvariantCultureIgnoreCase);
        }

        private AnsweredNotificationViewModel GetAnsweredNotificationModel(Answer answer)
        {
            var type = IsCurrentUser(answer.QuestionDetail.AskedBy.UserName) ? 
                AnsweredNotificationViewModel.AnswerNotificationTypeEnum.CurrentUserAnswer :
                AnsweredNotificationViewModel.AnswerNotificationTypeEnum.OtherAnswers;

            var model = new AnsweredNotificationViewModel()
            {
                AnswerId = answer.AnswerId,
                QuestionDetailId = answer.QuestionDetailId,
                TimeStamp = answer.TimeStamp,
                AnswerNotificationType = type,
                AnsweringUser = GetProfileFor(answer.User),
                AskingUser = GetProfileFor(answer.QuestionDetail.AskedBy)
            };
            return model;
        }

        private ProfileViewModel GetProfileFor(ApplicationUser user)
        {
            return _notificationsService.GetUserProfile(user.UserName);
        }

        private FollowedNotificationViewModel GetRelationshipNotificationModel(Relationship relationship)
        {
            var currentUser = _notificationsService.GetCurrentUserName();
            FollowedNotificationViewModel.FollowTypeEnum type = GetRelationshipNotificationType(relationship, currentUser);

            var model = new FollowedNotificationViewModel()
            {
                FollowedBy = GetProfileFor(relationship.SourceUser),
                UserFollowed = GetProfileFor(relationship.DestinationUser),
                TimeStamp = relationship.TimeStamp,
                FollowType = type
            };
            return model;
        }

        private FollowedNotificationViewModel.FollowTypeEnum GetRelationshipNotificationType(Relationship relationship, string currentUser)
        {
            var type = FollowedNotificationViewModel.FollowTypeEnum.NotCurerntUser;
            if (IsCurrentUser(relationship.SourceUser.UserName))
            {
                type = FollowedNotificationViewModel.FollowTypeEnum.CurrentUserFollowing;
            }
            else if (IsCurrentUser(relationship.DestinationUser.UserName))
            {
                type = FollowedNotificationViewModel.FollowTypeEnum.CurrentUserFollowed;
            }

            return type;
        }
    }
}