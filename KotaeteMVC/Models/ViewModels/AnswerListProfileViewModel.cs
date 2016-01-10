using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerListProfileViewModel : ItemListViewModel
    {
        public ProfileViewModel Profile { get; set; }

        public List<AnswerProfileViewModel> Answers { get; set; }

    }
}