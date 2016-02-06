using Resources;
using System.ComponentModel.DataAnnotations;

namespace KotaeteMVC.Models.ViewModels
{
    public class QuestionDetailViewModel
    {
        public string AskedToUserName { get; set; }

        [Required(ErrorMessageResourceName = "QuestionContentMissing", ErrorMessageResourceType = typeof(QuestionStrings))]
        public string QuestionContent { get; set; }

        [ScaffoldColumn(false)]
        public string AskedToScreenName { get; set; }
    }
}