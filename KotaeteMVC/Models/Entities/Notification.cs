using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.Entities
{
    public class Notification
    {
        public enum NotificationType
        {
            Comment = 1,
            Relationship = 2,
            Answer = 3,
            Question = 4,
            AnswerLike = 5
        }

        [Key]
        [Required]
        public int NotificationId { get; set; }

        [Required]
        public virtual NotificationType Type { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int EntityId { get; set; }
        public virtual bool Seen { get; set; }
        public virtual bool AllowNotificationAlert { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        

    }
}