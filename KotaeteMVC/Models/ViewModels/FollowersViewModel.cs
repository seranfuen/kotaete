using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class FollowersViewModel : PaginationViewModel
    {
        public ProfileViewModel OwnerProfile { get; set; }

        public List<ProfileViewModel> Followers { get; set; }
    }
}