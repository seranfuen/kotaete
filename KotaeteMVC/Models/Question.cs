using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class Question
    {
        [ScaffoldColumn(false)]
        public int QuestionId { get; set; }

        [ScaffoldColumn(false)]
        public ApplicationUser AskedBy { get; set; }

        [Required]
        public string Content { get; set; }

        [ScaffoldColumn(false)]
        public DateTime TimeStamp { get; set; }
    }
}