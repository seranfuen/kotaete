using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class AnsweredNotificationViewModel
    {
        public enum AnswerNotificationTypeEnum
        {
            CurrentUserAnswer,
            FollowingAnswer,
            OtherAnswers
        }
        public ProfileViewModel AnsweringUser { get; set; }
        public ProfileViewModel AskingUser { get; set; }
        public DateTime TimeStamp { get; set; }
        public int AnswerId { get; set; }
        public int QuestionDetailId { get; set; }
        public AnswerNotificationTypeEnum AnswerNotificationType { get; set; }
    }
}