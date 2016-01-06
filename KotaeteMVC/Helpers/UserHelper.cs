using KotaeteMVC.Controllers;
using KotaeteMVC.Models;
using System.Linq;
using System.Web.Mvc;
using System;

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

        public static bool ExistsUserName(this BaseController controller, string userName)
        {
            return controller.Context.Users.Any(user => user.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
        }


        public static ProfileQuestionViewModel GetProfileQuestionViewModel(this BaseController controller, string userName)
        {
            var currentUserName = controller.GetCurrentUserName();
            var currentUser = controller.Context.Users.FirstOrDefault(usr => usr.UserName.Equals(currentUserName, System.StringComparison.OrdinalIgnoreCase));
            var user = controller.Context.Users.First(usr => usr.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
            var profileQuestion = new ProfileQuestionViewModel()
            {
                Profile = InitializeProfile(controller, currentUser, user),
                QuestionDetail = InitializeQuestionDetail(user)
            };
            return profileQuestion;
        }

        private static ContentQuestionDetailViewModel InitializeQuestionDetail(ApplicationUser user)
        {
            return new ContentQuestionDetailViewModel()
            {
                AskedToScreenName = user.ScreenName,
                AskedToUserName = user.UserName
            };
        }

        public static ProfileViewModel GetProfile(this BaseController controller, string userName)
        {
            var profileUser = controller.GetUserWithName(userName);
            if (profileUser == null) return null;
            var currentUser = controller.GetCurrentUser();
            return controller.InitializeProfile(currentUser, profileUser);
        }

        public static ProfileViewModel InitializeProfile(this BaseController controller, ApplicationUser currentUser, ApplicationUser profileUser)
        {
            return new ProfileViewModel()
            {
                ScreenName = profileUser.ScreenName,
                FollowsYou = currentUser != null && profileUser.Following.Any(usr => usr.Equals(currentUser)),
                Following = currentUser != null && profileUser.Followers.Any(usr => usr.Equals(currentUser)),
                IsOwnProfile = currentUser != null && currentUser.UserName == profileUser.UserName,
                AvatarUrl = controller.GetAvatarUrl(profileUser),
                HeaderUrl = controller.GetHeaderUrl(profileUser),
                Bio = profileUser.Bio,
                Location = profileUser.Location,
                Homepage = profileUser.Homepage,
                User = profileUser,
                QuestionsReplied = GetQuestionsAnsweredByUser(controller, profileUser), // TODO: user should provide answers
                QuestionsAsked = GetQuestionsAnsweredToUser(controller, profileUser),
                FollowerCount = profileUser.Followers.Count(),
                FollowingCount = profileUser.Following.Count()
            };
        }

        private static int GetQuestionsAnsweredByUser(BaseController controller, ApplicationUser profileUser)
        {
            return controller.Context.Answers.Count(answer => answer.Deleted == false && answer.User.Id == profileUser.Id);
        }

        private static int GetQuestionsAnsweredToUser(BaseController controller, ApplicationUser profileUser)
        {
            return profileUser.QuestionsAsked.Count(qst => controller.Context.Answers.Any(answer => qst.QuestionDetailId == answer.QuestionDetailId && answer.Deleted == false));
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