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
    }
}