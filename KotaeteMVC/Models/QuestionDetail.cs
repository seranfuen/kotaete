using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotaeteMVC.Models
{
    public class QuestionDetail
    {
        [ScaffoldColumn(false)]
        public int QuestionDetailId { get; set; }

        [ScaffoldColumn(false)]
        public Question Question { get; set; }

        [InverseProperty("QuestionsAsked")]
        [ScaffoldColumn(false)]
        public ApplicationUser AskedBy { get; set; }

        [InverseProperty("QuestionsReceived")]
        [ScaffoldColumn(false)]
        public ApplicationUser AskedTo { get; set; }

        [ScaffoldColumn(false)]
        public DateTime TimeStamp { get; set; }
    }
}
