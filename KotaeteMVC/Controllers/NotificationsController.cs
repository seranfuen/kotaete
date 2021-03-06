﻿using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.NotificationModels;
using KotaeteMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

            var model = GetNotificationsListViewModel(currentUser, 20, true);

            return View("Notifications", model);
        }

        private NotificationsListViewModel GetNotificationsListViewModel(string userName, int notificationCount, bool showFriendAlerts)
        {
            var listNotifications = _notificationsService.GetLastUserNotifications(userName, notificationCount, false, false, showFriendAlerts);
            var profile = GetProfileFor(_notificationsService.GetCurrentUser());
            var dictionarySeen = listNotifications.ToDictionary(key => GetNotificationEntity(key), value => value.Seen);
            var lastDate = listNotifications.Max(not => not.TimeStamp);
            _notificationsService.UpdateSeenNotifications(lastDate);

            var model = new NotificationsListViewModel(dictionarySeen, profile);
            return model;
        }

        public ActionResult Notification(IEventEntity eventEntity, bool seen)
        {
            if (eventEntity is Comment)
            {
                return GetCommentNotificationPartialView(eventEntity as Comment, seen);
            }
            else if (eventEntity is Answer)
            {
                return GetAnswerNotificationPartialView(eventEntity as Answer, seen);
            }
            else if (eventEntity is AnswerLike)
            {
                return GetAnswerLikedNotificationPartialView(eventEntity as AnswerLike, seen);
            }
            else if (eventEntity is QuestionDetail)
            {
                return GetQuestionAskedNotificationPartialView(eventEntity as QuestionDetail, seen);
            }
            else if (eventEntity is Relationship)
            {
                return GetRelationshipNotificationPartialView(eventEntity as Relationship, seen);
            }
            else
            {
                return null;
            }
        }

        private AnsweredNotificationViewModel GetAnsweredNotificationModel(Answer answer, bool seen)
        {
            var type = AnsweredNotificationViewModel.AnswerNotificationTypeEnum.OtherAnswers;
            if (IsCurrentUser(answer.QuestionDetail.AskedBy.UserName))
            {
                type = AnsweredNotificationViewModel.AnswerNotificationTypeEnum.CurrentUserAnswer;
            } else if (_notificationsService.IsCurrentFollowing(answer.User.UserName))
            {
                type = AnsweredNotificationViewModel.AnswerNotificationTypeEnum.FollowingAnswer;
            }

            var model = new AnsweredNotificationViewModel(answer, seen)
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

        private ActionResult GetAnswerLikedNotificationPartialView(AnswerLike answerLike, bool seen)
        {
            var model = GetAnswerLikeNotificationModel(answerLike, seen);
            return PartialView("AnswerLikeNotification", model);
        }

        private AnswerLikeNotificationViewModel GetAnswerLikeNotificationModel(AnswerLike like, bool seen)
        {
            var type = IsCurrentUser(like.Answer.User.UserName) ? AnswerLikeNotificationViewModel.AnswerLikeNotificationTypeEnum.CurrentUserAnswer :
                AnswerLikeNotificationViewModel.AnswerLikeNotificationTypeEnum.OtherUsers;

            var model = new AnswerLikeNotificationViewModel(like, seen)
            {
                AnswerId = like.AnswerId,
                AnswerLikeNotificationType = type,
                AnswerUser = GetProfileFor(like.Answer.User),
                LikingUser = GetProfileFor(like.ApplicationUser),
                TimeStamp = like.TimeStamp
            };
            return model;
        }

        private ActionResult GetAnswerNotificationPartialView(Answer answer, bool seen)
        {
            var model = GetAnsweredNotificationModel(answer, seen);
            return PartialView("AnswerNotification", model);
        }

        private CommentNotificationViewModel GetCommentNotificationModel(Comment comment, bool seen)
        {
            var type = GetCommentNotificationType(comment);
            var model = new CommentNotificationViewModel(comment, seen)
            {
                AnswerId = comment.AnswerId,
                AnsweringUser = GetProfileFor(comment.Answer.User),
                CommentingUser = GetProfileFor(comment.User),
                CommentNotificationType = type,
                TimeStamp = comment.TimeStamp
            };
            return model;
        }

        private ActionResult GetCommentNotificationPartialView(Comment comment, bool seen)
        {
            var model = GetCommentNotificationModel(comment, seen);
            return PartialView("CommentNotification", model);
        }

        private CommentNotificationViewModel.CommentNotificationTypeEnum GetCommentNotificationType(Comment comment)
        {
            if (IsCurrentUser(comment.Answer.User) && !IsCurrentUser(comment.User))
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CurrentUserAnswer;
            }
            else if (IsCurrentUser(comment.Answer.QuestionDetail.AskedBy.UserName) && !IsCurrentUser(comment.User))
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CurrentUserAskedQuestion;
            }
            else if (comment.User == comment.Answer.User)
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CommentingUserIsAnsweringUser;
            }
            else
            {
                return CommentNotificationViewModel.CommentNotificationTypeEnum.CurrentUserCommentedAnswer;
            }
        }

        private IEventEntity GetNotificationEntity(Notification notification)
        {
            IEventEntity entity = null;
            if (notification.Type == Models.Entities.Notification.NotificationType.Answer)
            {
                entity = Context.Answers.First(answ => answ.AnswerId == notification.EntityId);
            }
            else if (notification.Type == Models.Entities.Notification.NotificationType.AnswerLike)
            {
                entity = Context.AnswerLikes.First(like => like.AnswerLikeId == notification.EntityId);
            }
            else if (notification.Type == Models.Entities.Notification.NotificationType.Comment)
            {
                entity = Context.Comments.First(cmnt => cmnt.CommentId == notification.EntityId);
            }
            else if (notification.Type == Models.Entities.Notification.NotificationType.Question)
            {
                entity = Context.QuestionDetails.First(qstDetail => qstDetail.QuestionDetailId == notification.EntityId);
            }
            else if (notification.Type == Models.Entities.Notification.NotificationType.Relationship)
            {
                entity = Context.Relationships.First(rel => rel.RelationshipId == notification.EntityId);
            }

            return entity;
        }

        private ProfileViewModel GetProfileFor(ApplicationUser user)
        {
            return _notificationsService.GetUserProfile(user.UserName);
        }

        private QuestionAskedNotificationViewModel GetQuestionAskedNotificationModel(QuestionDetail questionDetail, bool seen)
        {
            var model = new QuestionAskedNotificationViewModel(questionDetail, seen)
            {
                AskedUser = GetProfileFor(questionDetail.AskedTo),
                AskingUser = GetProfileFor(questionDetail.AskedBy),
                TimeStamp = questionDetail.TimeStamp,
                QuestionDetailId = questionDetail.QuestionDetailId
            };
            return model;
        }

        private ActionResult GetQuestionAskedNotificationPartialView(QuestionDetail questionDetail, bool seen)
        {
            var model = GetQuestionAskedNotificationModel(questionDetail, seen);
            return PartialView("QuestionNotification", model);
        }

        private FollowedNotificationViewModel GetRelationshipNotificationModel(Relationship relationship, bool seen)
        {
            var currentUser = _notificationsService.GetCurrentUserName();
            FollowedNotificationViewModel.FollowTypeEnum type = GetRelationshipNotificationType(relationship, currentUser);

            var model = new FollowedNotificationViewModel(relationship, seen)
            {
                FollowedBy = GetProfileFor(relationship.SourceUser),
                UserFollowed = GetProfileFor(relationship.DestinationUser),
                TimeStamp = relationship.TimeStamp,
                FollowType = type
            };
            return model;
        }

        private ActionResult GetRelationshipNotificationPartialView(Relationship relationship, bool seen)
        {
            var model = GetRelationshipNotificationModel(relationship, seen);
            return PartialView("RelationshipNotification", model);
        }
        private FollowedNotificationViewModel.FollowTypeEnum GetRelationshipNotificationType(Relationship relationship, string currentUser)
        {
            var type = FollowedNotificationViewModel.FollowTypeEnum.NotCurrentUser;
            if (IsCurrentUser(relationship.SourceUser.UserName))
            {
                type = FollowedNotificationViewModel.FollowTypeEnum.CurrentUserFollowing;
            }
            else if (IsCurrentUser(relationship.DestinationUser.UserName))
            {
                type = FollowedNotificationViewModel.FollowTypeEnum.CurrentUserFollowed;
            }
            else if (_notificationsService.IsCurrentFollowing(relationship.SourceUser.UserName) || _notificationsService.IsCurrentFollowing(relationship.DestinationUser.UserName))
            {
                type = FollowedNotificationViewModel.FollowTypeEnum.FriendRelationship;
            }
            return type;
        }

        private bool IsCurrentUser(ApplicationUser user)
        {
            return IsCurrentUser(user.UserName);
        }
        private bool IsCurrentUser(string userName)
        {
            var currentUser = _notificationsService.GetCurrentUserName();
            return userName.Equals(currentUser, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}