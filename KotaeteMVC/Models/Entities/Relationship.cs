using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KotaeteMVC.Models.Entities
{
    public enum RelationshipType
    {
        Friendship,
        Block
    }

    public class Relationship : IEventEntity
    {
        public virtual int RelationshipId { get; set; }
        public virtual DateTime TimeStamp { get; set; }

        [DefaultValue(true)]
        public bool Active { get; set; }

        [Required]
        [ForeignKey("DestinationUserId")]
        public virtual ApplicationUser SourceUser { get; set; }

        [Required]
        [ForeignKey("SourceUserId")]
        public virtual ApplicationUser DestinationUser { get; set; }

        public virtual string DestinationUserId { get; set; }
        public virtual string SourceUserId { get; set; }

        [Required]
        public virtual RelationshipType RelationshipType { get; set; }

        public void AddNotifications()
        {
            SourceUser.Notifications.Add(new Notification()
            {
                AllowNotificationAlert = false,
                EntityId = RelationshipId,
                Seen = false,
                TimeStamp = TimeStamp,
                User = SourceUser,
                UserId = SourceUser.Id,
                Type = Notification.NotificationType.Relationship
            });
            DestinationUser.Notifications.Add(new Notification()
            {
                AllowNotificationAlert = true,
                EntityId = RelationshipId,
                Seen = false,
                TimeStamp = TimeStamp,
                User = DestinationUser,
                UserId = DestinationUser.Id,
                Type = Notification.NotificationType.Relationship
            });
        }
    }
}