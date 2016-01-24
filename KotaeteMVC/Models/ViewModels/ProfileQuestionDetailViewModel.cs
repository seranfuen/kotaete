using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class ProfileQuestionDetailViewModel
    {
        public virtual ProfileViewModel Profile { get; set; }

        public virtual List<QuestionDetailAnswerViewModel> QuestionDetails { get; set; }
    }
}