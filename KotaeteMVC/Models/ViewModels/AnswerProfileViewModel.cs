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

        public MoreButtonViewModel CommentsMoreButton { get; set; }

        public List<string> QuestionParagraphs { set; get; }

        public List<string> AnswerParagraphs { set; get; }

        public AnswerLikeViewModel LikesModel { get; set; }

        public int TotalComments { get; set; }

        public bool ShowingFullDetail { get; set; }
    }
}