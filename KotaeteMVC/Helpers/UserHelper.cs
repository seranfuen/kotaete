using KotaeteMVC.Controllers;
using KotaeteMVC.Models;
using System.Linq;
using System.Web.Mvc;

namespace KotaeteMVC.Helpers
{
    public static class UserHelpers
    {
        public static ApplicationUser GetCurrentUser(this BaseController controller)
        {
            var currentUserName = controller.GetCurrentUserName();
            return controller.Context.Users.FirstOrDefault(usr => usr.UserName == currentUserName);
        }

        public static string GetCurrentUserName(this Controller controller)
        {
            return controller.HttpContext.User.Identity.Name;
        }

        public static ApplicationUser GetUserWithName(this BaseController controller, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            return controller.Context.Users.FirstOrDefault(usr => usr.UserName == userName);
        }


        public static ProfileQuestionViewModel GetProfileQuestionViewModel(this BaseController controller, string userName)
        {
            var currentUserName = controller.GetCurrentUserName();
            var currentUser = controller.Context.Users.FirstOrDefault(usr => usr.UserName.Equals(currentUserName, System.StringComparison.OrdinalIgnoreCase));
            var user = controller.Context.Users.First(usr => usr.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
            var profile = new ProfileQuestionViewModel()
            {
                ScreenName = user.ScreenName,
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
                FollowingCount = user.Following.Count(),
                QuestionDetail = new ContentQuestionDetailViewModel()
                {
                    AskedToScreenName = user.ScreenName,
                    AskedToUserName = user.UserName
                }
            };
            return profile;
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