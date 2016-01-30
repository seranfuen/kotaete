namespace KotaeteMVC.Models.ViewModels
{
    public class FollowButtonViewModel
    {
        public bool IsOwnProfile { set; get; }
        public bool IsFollowing { set; get; }
        public string UserName { set; get; }
        public bool IsUserAuthenticated { set; get; }
        public string SuccessFollowMessage { get; set; }
        public string FailureMessage { get; set; }
    }
}