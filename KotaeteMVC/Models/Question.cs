using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class Question
    {
        [ScaffoldColumn(false)]
        public virtual int QuestionId { get; set; }

        [ScaffoldColumn(false)]
        public virtual ApplicationUser AskedBy { get; set; }

        [Required]
        [DisplayName("Question")]
        public virtual string Content { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }
    }
}