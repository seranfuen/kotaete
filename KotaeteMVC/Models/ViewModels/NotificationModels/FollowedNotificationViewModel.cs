using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class FollowedNotificationViewModel
    {
        public enum FollowTypeEnum
        {
            CurrentUserFollowing,
            CurrentUserFollowed,
            FriendRelationship,
            NotCurrentUser
        }

        public ProfileViewModel FollowedBy { get; set; }

        public ProfileViewModel UserFollowed { get; set; }

        public DateTime TimeStamp { get; set; }

        public FollowTypeEnum FollowType { get; set; }
    }
}