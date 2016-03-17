using KotaeteMVC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels
{
    public class FollowerNotificationViewModel
    {
        public ProfileViewModel FollowingUser { get; set; }
        public ProfileViewModel FollowedUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
