using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotaeteMVC.Models.ViewModels
{
    public class InboxViewModel : PaginationViewModel
    {
        public virtual ProfileViewModel Profile { get; set; }

        public virtual List<QuestionDetailAnswerViewModel> QuestionDetails { get; set; }
    }
}
