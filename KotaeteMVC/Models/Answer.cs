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
        public int AnswerId { get; set; }

        [Display(Name = "Your answer")]
        [Required]
        public string Content { get; set; }

        [ScaffoldColumn(false)]
        public ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public QuestionDetail Question { get; set; }

        [ScaffoldColumn(false)]
        public DateTime TimeStamp { get; set; }

    }
}
