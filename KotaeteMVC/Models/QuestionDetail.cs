using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public virtual int QuestionDetailId { get; set; }

        [ScaffoldColumn(false)]
        public virtual Question Question { get; set; }

        [InverseProperty("QuestionsAsked")]
        [ScaffoldColumn(false)]
        public virtual ApplicationUser AskedBy { get; set; }

        [InverseProperty("QuestionsReceived")]
        [ScaffoldColumn(false)]
        public virtual ApplicationUser AskedTo { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool Deleted { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool SeenByUser { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool Answered { get; set; }
    }
}
