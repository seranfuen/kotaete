using KotaeteMVC.Controllers;
using KotaeteMVC.Models.Entities;
using System.Linq;

namespace KotaeteMVC.Helpers
{
    public static class UserHelpers
    {
        public static ApplicationUser GetUserWithName(this BaseController controller, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            return controller.Context.Users.FirstOrDefault(usr => usr.UserName == userName);
        }

        public static bool ExistsUserName(this BaseController controller, string userName)
        {
            return controller.Context.Users.Any(user => user.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
        }

        private static int GetQuestionsAnsweredByUser(BaseController controller, ApplicationUser profileUser)
        {
            return controller.Context.Answers.Count(answer => answer.Active == false && answer.User.Id == profileUser.Id);
        }

        private static int GetQuestionsAnsweredToUser(BaseController controller, ApplicationUser profileUser)
        {
            return profileUser.QuestionsAsked.Count(qst => controller.Context.Answers.Any(answer => qst.QuestionDetailId == answer.QuestionDetailId && answer.Active == false));
        }

        public static bool HasUserName(this ApplicationUser user, string userName)
        {
            return user.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}