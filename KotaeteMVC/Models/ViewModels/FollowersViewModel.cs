using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using System.Collections.Generic;

namespace KotaeteMVC.Models
{
    public class FollowersViewModel : PaginationViewModel
    {
        public ProfileViewModel OwnerProfile { get; set; }

        public List<ProfileViewModel> Followers { get; set; }
    }
}