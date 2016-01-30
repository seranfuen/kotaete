using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.Entities
{
    public class AnswerLike
    {
        public virtual int AnswerLikeId { get; set; }
        public virtual int AnswerId { get; set; }
        [Required]
        public virtual Answer Answer { get; set; }
        [Required]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual string ApplicationUserId { get; set; }
        [Required]
        public virtual bool Active { get; set; }
        [Required]
        public virtual DateTime TimeStamp { get; set; }
    }
}