using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class QuestionDetailAnswerViewModel
    {
        public virtual QuestionDetail QuestionDetail { get; set; }

		public virtual Answer Answer { get; set; }

		public virtual string AskerAvatarUrl { get; set; }

        public virtual string AskedTimeAgo { get; set; }

        public virtual List<string> QuestionParagraphs { get; set; }

        public virtual bool Seen { get; set; }
	}
}