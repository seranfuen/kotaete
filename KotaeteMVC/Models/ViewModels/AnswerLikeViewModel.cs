namespace KotaeteMVC.Models.ViewModels
{
    public class AnswerLikeViewModel
    {
        public int LikeCount { get; set; }

        public bool HasLiked { get; set; }

        public int AnswerId { get; set; }
    }
}