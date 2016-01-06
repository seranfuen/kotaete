using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class ProfileAnswersViewModel
    {
        public virtual ProfileQuestionViewModel Profile { get; set; }

        public virtual List<AnswerViewModel> Answers { get; set; }
    }
}