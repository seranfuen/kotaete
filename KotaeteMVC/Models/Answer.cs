using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotaeteMVC.Models
{
    public class Answer
    {
        [ScaffoldColumn(false)]
        public virtual int AnswerId { get; set; }

        [Display(Name = "Your answer")]
        [Required]
        public virtual string Content { get; set; }

        [ScaffoldColumn(false)]
        public virtual ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public virtual QuestionDetail Question { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }

    }
}
