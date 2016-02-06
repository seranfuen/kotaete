using KotaeteMVC.Models.ViewModels;

namespace KotaeteMVC.Models
{
    /// <summary>
    /// View Model that binds a user id that gets a question asked and a question content
    /// </summary>
    public class ProfileQuestionViewModel
    {
        public QuestionDetailViewModel QuestionDetail { get; set; }
        public ProfileViewModel Profile { get; set; }
    }
}