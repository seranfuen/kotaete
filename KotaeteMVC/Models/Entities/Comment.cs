using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.Entities
{
    public class Comment : IEventEntity
    {
        public virtual int CommentId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual string UserId { get; set; }
        public virtual Answer Answer { get; set; }
        public virtual int AnswerId { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        [DefaultValue(true)]
        public virtual bool Active { get; set; }
        public virtual string Content { get; set; }

        public void AddNotifications()
        {
            var commentingUsers = Answer.Comments.Select(comment => comment.User).ToList();
            commentingUsers.Add(Answer.User);
            commentingUsers.Add(Answer.QuestionDetail.AskedBy);
            var query = from user in commentingUsers
                        where user != User
                        select user;
            query.Distinct().ToList().ForEach(AddNotification);
        }

        private void AddNotification(ApplicationUser user)
        {
            user.Notifications.Add(new Notification()
            {
                 AllowNotificationAlert = true,
                 EntityId = CommentId,
                 User = user,
                 TimeStamp = TimeStamp,
                 Type = Notification.NotificationType.Comment,
                 Seen = false
            });
        }
    }
}