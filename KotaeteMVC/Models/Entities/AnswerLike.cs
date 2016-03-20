using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KotaeteMVC.Models.Entities
{
    public class AnswerLike : IEventEntity
    {
        public virtual int AnswerLikeId { get; set; }
        public virtual int AnswerId { get; set; }

        [Required]
        public virtual Answer Answer { get; set; }

        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual string ApplicationUserId { get; set; }

        [Required]
        [DefaultValue(true)]
        public virtual bool Active { get; set; }

        [Required]
        public virtual DateTime TimeStamp { get; set; }
    }
}