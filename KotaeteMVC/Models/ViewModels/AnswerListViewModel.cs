using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerListViewModel : PaginationViewModel
    {
        public List<AnswerProfileViewModel> Answers;
    }
}