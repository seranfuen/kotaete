using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace KotaeteMVC.Controllers
{
    public class UserController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string userName, string request = "")
        {
            var user = GetUserWithScreenName(userName);
            if (user == null)
            {
                ViewBag.UserName = userName;
                return View("UserNotFound");
            }
            if (request == "Followers")
            {
                return View("Followers", user);
            } else if (request == "Following")
            {
                return View("Following", user);
            }
            var currentUser = GetCurrentUser();
            var userProfile = new ProfileQuestionViewModel()
            {
                ProfileUserName = userName,
                FollowsYou = currentUser != null && user.Following.Any(usr => usr.Equals(currentUser)),
                CurrentUserAuthenticated = currentUser != null,
                AvatarUrl = GetAvatarUrl(user),
                Bio = user.Bio,
                Location = user.Location,
                Homepage = user.Homepage
            };
            return View(userProfile);
        }

        private string GetAvatarUrl(ApplicationUser user)
        {
            var url = "Images/Avatars/";
            if (user.Avatar != null)
            {
                return url + user.Avatar;
            }
            return url + "anonymous.jpg";
        }

        private ApplicationUser GetUserWithScreenName(string screenName)
        {
            return db.Users.FirstOrDefault(usr => usr.ScreenName == screenName);
        }

        [Authorize]
        public ActionResult AskQuestion([Bind(Include = "AskedUserName, QuestionContent")] ProfileQuestionViewModel question)
        {
            var askedUser = GetUserWithScreenName(question.ProfileUserName);
            ApplicationUser asker = GetCurrentUser();

            var now = DateTime.Now;

            var qstDetail = new QuestionDetail()
            {
                Question = new Question()
                {
                    Content = question.QuestionContent,
                    TimeStamp = now,
                    AskedBy = asker,
                },
                AskedTo = askedUser,
                AskedBy = asker,
                TimeStamp = now
            };
            var result = TryValidateModel(qstDetail) && TryValidateModel(qstDetail.Question);
            if (result)
            {

                db.QuestionDetails.Add(qstDetail);

                db.SaveChanges();
                return View("Index", question.ProfileUserName);
            }
            return View("Index", question);
        }

        private ApplicationUser GetCurrentUser()
        {
            return db.Users.FirstOrDefault(usr => usr.UserName == HttpContext.User.Identity.Name);
        }

    }
}