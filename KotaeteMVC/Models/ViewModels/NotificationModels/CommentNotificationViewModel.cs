using System;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class CommentNotificationViewModel : NotificationViewModel
    {
        public CommentNotificationViewModel(object entity, bool seen) : base(entity, seen)
        {
        }

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