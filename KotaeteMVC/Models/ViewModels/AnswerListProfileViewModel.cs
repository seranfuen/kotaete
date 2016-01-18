using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerListProfileViewModel
    {

        public AnswerListProfileViewModel(List<AnswerProfileViewModel> answers)
        {
            AnswerList = new AnswerListViewModel(answers);
        }

        public ProfileViewModel Profile { get; set; }

        public AnswerListViewModel AnswerList { get; private set; }

    }
}