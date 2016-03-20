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
            Comment,
            Relationship,
            Answer,
            Question,
            AnswerLike
        }

        [Required]
        public virtual NotificationType Type { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public virtual int UserId { get; set; }
        [Required]
        public virtual int EntityId { get; set; }
        public virtual bool Seen { get; set; }
        public virtual bool AllowNotificationAlert { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        

    }
}