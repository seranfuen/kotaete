using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class QuestionAskedNotificationViewModel : NotificationViewModel
    {
        public QuestionAskedNotificationViewModel(object entity, bool seen) : base(entity, seen)
        {
        }

        public ProfileViewModel AskingUser { get; set; }
        public ProfileViewModel AskedUser { get; set; }
        public int QuestionDetailId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}