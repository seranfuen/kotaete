using System;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class FollowedNotificationViewModel : NotificationViewModel
    {
        public FollowedNotificationViewModel(object entity, bool seen) : base(entity, seen)
        {
        }

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