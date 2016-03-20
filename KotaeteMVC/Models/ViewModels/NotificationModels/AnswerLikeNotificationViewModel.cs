using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class AnswerLikeNotificationViewModel
    {
        public enum AnswerLikeNotificationTypeEnum
        {
            CurrentUserAnswer,
            OtherUsers
        }

        public int AnswerId { get; set; }
        public ProfileViewModel LikingUser { get; set; }
        public ProfileViewModel AnswerUser { get; set; }
        public DateTime TimeStamp { get; set; }
        public AnswerLikeNotificationTypeEnum AnswerLikeNotificationType { get; set; }

    }
}