using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class AnswerListProfileViewModel
    {
        public ProfileViewModel Profile { get; set; }

        public List<AnswerProfileViewModel> Answers { get; set; }
    }
}