using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class NoAnswersProfileViewModel
    {
        public ProfileViewModel Profile { set; get; }
        public bool IsAnswers { set; get; }
    }
}