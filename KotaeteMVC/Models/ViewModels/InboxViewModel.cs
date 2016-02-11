using KotaeteMVC.Models.ViewModels.Base;
using System.Collections.Generic;

namespace KotaeteMVC.Models.ViewModels
{
    public class InboxViewModel : PaginationViewModel
    {
        public virtual ProfileViewModel Profile { get; set; }

        public virtual List<QuestionDetailAnswerViewModel> QuestionDetails { get; set; }

        public ConfirmModalViewModel ConfirmModal { get; set; }
    }
}