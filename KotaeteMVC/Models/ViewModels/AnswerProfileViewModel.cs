using KotaeteMVC.Models.Entities;
using System.Collections.Generic;

namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerProfileViewModel
    {
        public string AskerAvatarUrl { get; set; }

        public string ReplierAvatarUrl { get; set; }

        public Answer Answer { get; set; }

        public List<CommentViewModel> Comments { get; set; }

        public List<string> QuestionParagraphs { set; get; }

        public List<string> AnswerParagraphs { set; get; }

        public string AskedTimeAgo { get; set; }

        public string RepliedTimeAgo { get; set; }

        public AnswerLikeViewModel LikesModel { get; set; }
    }
}