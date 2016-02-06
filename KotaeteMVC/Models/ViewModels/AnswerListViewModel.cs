using KotaeteMVC.Models.ViewModels.Base;
using System.Collections.Generic;

namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerListViewModel : PaginationViewModel
    {
        public List<AnswerProfileViewModel> Answers { get; set; }
    }
}