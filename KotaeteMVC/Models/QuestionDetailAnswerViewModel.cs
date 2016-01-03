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

        [ScaffoldColumn(false)]
        public virtual int QuestionDetailId { get; set; }

        [Required]
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