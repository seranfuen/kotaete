using System;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class CommentNotificationViewModel
    {
        public enum CommentNotificationTypeEnum
        {
            CurrentUserAnswer,
            CurrentUserCommentedAnswer,
            CommentingUserIsAnsweringUser,
            CurrentUserAskedQuestion,
        }

        public int AnswerId { get; set; }
        public ProfileViewModel CommentingUser { get; set; }
        public ProfileViewModel AnsweringUser { get; set; }
        public CommentNotificationTypeEnum CommentNotificationType { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}