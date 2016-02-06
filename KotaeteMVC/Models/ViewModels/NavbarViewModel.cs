namespace KotaeteMVC.Models.ViewModels
{
    public class NavbarViewModel
    {
        public bool IsAuthenticated { get; set; }

        public int InboxCount { get; set; }

        public string AvatarUrl { get; set; }

        public string UserName { get; set; }
    }
}