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
    public class UserController : AlertControllerBase
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("user/{userName}")]
        [Route("user/{userName}/{request}", Name = "userNameRequest")]
        public ActionResult Index(string userName, string request = "")
        {
            var user = GetUserWithName(userName);
            if (user == null)
            {
                ViewBag.UserName = userName;
                return View("UserNotFound");
            }

            if (request == "Followers")
            {
                return View("Followers", GetFollowersViewModel(user));
            }
            else if (request == "Following")
            {
                return View("Following", GetFollowingViewModel(user));
            }
            var currentUser = GetCurrentUser();
            ProfileQuestionViewModel userProfile = GetProfileQuestionViewModel(userName);
            return View(userProfile);
        }

        private FollowersViewModel GetFollowersViewModel(ApplicationUser user)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = GetProfileQuestionViewModel(user.UserName);
            followers.Followers = user.Followers.Select(follower => GetProfileQuestionViewModel(follower.UserName)).ToList();
            return followers;
        }

        private FollowersViewModel GetFollowingViewModel(ApplicationUser user)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = GetProfileQuestionViewModel(user.UserName);
            followers.Followers = user.Following.Select(following => GetProfileQuestionViewModel(following.UserName)).ToList();
            return followers;
        }


        private ProfileQuestionViewModel GetProfileQuestionViewModel(string profileUserName)
        {
            var currentUser = GetCurrentUser();
            var user = GetUserWithName(profileUserName);
            var profile = new ProfileQuestionViewModel()
            {
                ProfileUserName = user.ScreenName,
                FollowsYou = currentUser != null && user.Following.Any(usr => usr.Equals(currentUser)),
                Following = currentUser != null && user.Followers.Any(usr => usr.Equals(currentUser)),
                IsOwnProfile = currentUser != null && currentUser.Equals(user),
                CurrentUserAuthenticated = currentUser != null,
                AvatarUrl = GetAvatarUrl(user),
                HeaderUrl = GetHeaderUrl(user),
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

        private string GetAvatarUrl(ApplicationUser user)
        {
            var url = "/Images/Avatars/";
            if (user.Avatar != null)
            {
                return url + user.Avatar;
            }
            return url + "anonymous.jpg";
        }

        private string GetHeaderUrl(ApplicationUser user)
        {
            var url = "/Images/Headers/";
            if (user.Header != null)
            {
                return url + user.Header;
            }
            return null;
        }


        private ApplicationUser GetUserWithName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            return db.Users.FirstOrDefault(usr => usr.UserName == userName);
        }

        [Authorize]
        public ActionResult AskQuestion([Bind(Include = "AskedUserName, QuestionContent")] ProfileQuestionViewModel question)
        {
            var askedUser = GetUserWithName(question.ProfileUserName);
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

        [Route("follow")]
        [Authorize]
        public ActionResult FollowUnfollowUser([Bind(Include = "UserToFollowName, Action")] FollowUserViewModel followRequest)
        {
            var userToFollow = GetUserWithName(followRequest.UserToFollowName);
            if (userToFollow == null)
            {
                return new HttpUnauthorizedResult("The user " + followRequest.UserToFollowName + " does not exist");
            }
            var current = GetCurrentUser();
            if (followRequest.Action == "Follow")
            {
                if (current.Following.Any(usr => usr.Equals(userToFollow)))
                {
                    AddAlertDanger("You are already following this user", "Cannot follow", true);
                } else
                {
                    current.Following.Add(userToFollow);
                    AddAlertSuccess("You are now following " + userToFollow.ScreenName, "Success", true);
                }
            } else if (followRequest.Action == "Unfollow")
            {
                if (current.Following.Any(usr => usr.Equals(userToFollow)) == false)
                {
                    AddAlertDanger("You were not following this user", "Cannot unfollow", true);
                } else
                {
                    current.Following.Remove(userToFollow);
                    AddAlertSuccess(userToFollow.ScreenName + " was successfully unfollowed", "Success", true);
                }
            } else
            {
                return new HttpUnauthorizedResult("Invalid action");
            }
            try
            {
                db.SaveChanges();
            } catch (Exception e)
            {
                AddAlertDangerOverride("There was an error saving the changes: " + e.Message, "Critical");
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        private ApplicationUser GetCurrentUser()
        {
            return db.Users.FirstOrDefault(usr => usr.UserName == HttpContext.User.Identity.Name);
        }

    }
}