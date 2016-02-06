using System.Collections.Generic;

namespace KotaeteMVC.Models.ViewModels
{
    public class ProfileQuestionDetailViewModel
    {
        public virtual ProfileViewModel Profile { get; set; }

        public virtual List<QuestionDetailAnswerViewModel> QuestionDetails { get; set; }
    }
}