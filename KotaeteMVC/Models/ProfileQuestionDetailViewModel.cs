using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class ProfileQuestionDetailViewModel
    {
        public virtual ProfileQuestionViewModel Profile { get; set; }

        public virtual List<QuestionDetailAnswerViewModel> QuestionDetails { get; set; }
    }
}