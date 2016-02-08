using KotaeteMVC.App_GlobalResources;
using KotaeteMVC.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KotaeteMVC.Models.ViewModels
{
    public class CommentViewModel
    {
        [Required(ErrorMessageResourceType = typeof(AnswerStrings), ErrorMessageResourceName = "CommentRequired")]
        public string Content { get; set; }
        public Comment Comment { get; set; }
        public string UserName { get; set; }
        public string ScreenName { get; set; }
        public string TimeAgo { get; set; }
        public List<string> CommentParagraphs { get; set; }
        public string AvatarUrl { get; set; }
        public int AnswerId { get; set; }
    }
}
