using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Helpers
{
    public static class UserHelpers
    {
        public static ApplicationUser GetCurrentUser(this Controller controller)
        {
            var db = new ApplicationDbContext();
            var currentUserName = controller.GetCurrentUserName();
            return db.Users.FirstOrDefault(usr => usr.UserName == currentUserName);
        }

        public static string GetCurrentUserName(this Controller controller)
        {
            return controller.HttpContext.User.Identity.Name;
        }

        public static ApplicationUser GetUserWithName(this Controller controller, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(usr => usr.UserName == userName);
        }


        public static ProfileQuestionViewModel GetProfileQuestionViewModel(this Controller controller, string profileUserName)
        {
            using (var db = new ApplicationDbContext())
            {
                var currentUserName = controller.GetCurrentUserName();
                var currentUser = db.Users.First(usr => usr.UserName == currentUserName);
                var user = db.Users.First(usr => usr.UserName == profileUserName);
                var profile = new ProfileQuestionViewModel()
                {
                    ProfileUserName = user.ScreenName,
                    FollowsYou = currentUser != null && user.Following.Any(usr => usr.Equals(currentUser)),
                    Following = currentUser != null && user.Followers.Any(usr => usr.Equals(currentUser)),
                    IsOwnProfile = currentUser != null && currentUser.UserName == user.UserName,
                    CurrentUserAuthenticated = currentUser != null,
                    AvatarUrl = controller.GetAvatarUrl(user),
                    HeaderUrl = controller.GetHeaderUrl(user),
                    Bio = user.Bio,
                    Location = user.Location,
                    Homepage = user.Homepage,
                    User = user,
                    QuestionsReplied = 0, // TODO: user should provide answers
                    QuestionsAsked = user.QuestionsAsked.Count(),
                    FollowerCount = user.Followers.Count(),
                    FollowingCount = user.Following.Count()
                };
                return profile;
            }
        }

        public static string GetAvatarUrl(this Controller controller, ApplicationUser user)
        {
            var url = "/Images/Avatars/";
            if (user.Avatar != null)
            {
                return url + user.Avatar;
            }
            return url + "anonymous.jpg";
        }

        public static string GetHeaderUrl(this Controller controller, ApplicationUser user)
        {
            var url = "/Images/Headers/";
            if (user.Header != null)
            {
                return url + user.Header;
            }
            return null;
        }
    }
}