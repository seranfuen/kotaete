using KotaeteMVC.App_GlobalResources;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class QuestionDetailAnswerViewModel
    {
        [ScaffoldColumn(false)]
        public virtual QuestionDetail QuestionDetail { get; set; }

        [Required]
        public virtual int QuestionDetailId { get; set; }

        [Display(ResourceType = typeof(AnswerStrings), Name = "AnswerFieldName")]
        [Required]
        [MaxLength(1400, ErrorMessageResourceName = "AnswerMaxLengthError", ErrorMessageResourceType = typeof(AnswerStrings))]
        public virtual string AnswerContent { get; set; }

        [ScaffoldColumn(false)]
		public virtual string AskerAvatarUrl { get; set; }

        [ScaffoldColumn(false)]
        public virtual string AskedTimeAgo { get; set; }

        [ScaffoldColumn(false)] 
        public virtual List<string> QuestionParagraphs { get; set; }

        [ScaffoldColumn(false)]
        public virtual bool Seen { get; set; }
	}
}