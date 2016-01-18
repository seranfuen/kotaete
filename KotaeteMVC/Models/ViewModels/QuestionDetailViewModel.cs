using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class QuestionDetailViewModel
    {

        public string AskedToUserName { get; set; }

        public bool AskToAllFollowers { get; set; }

        [Required(ErrorMessageResourceName = "QuestionContentMissing", ErrorMessageResourceType =typeof(MainGlobal))]
        public string QuestionContent { get; set; }

        [ScaffoldColumn(false)]
        public string AskedToScreenName { get; set; }
    }
}