using KotaeteMVC.App_GlobalResources;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Display(ResourceType = typeof(AnswerStrings), Name = "YourAnswer")]
        [Required]
        [MaxLength(1400)]
        public virtual string Content { get; set; }

        [ScaffoldColumn(false)]
        public virtual ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public virtual int QuestionDetailId { get; set; }

        [ScaffoldColumn(false)]
        public virtual QuestionDetail Question { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool Deleted { get; set; }

    }
}
