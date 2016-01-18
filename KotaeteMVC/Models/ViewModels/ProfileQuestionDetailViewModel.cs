using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class ProfileQuestionDetailViewModel : QuestionDetailViewModel
    {
        public ProfileViewModel Profile { get; set; }
    }
}