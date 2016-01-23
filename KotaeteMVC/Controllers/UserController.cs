using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Helpers;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Resources;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;

namespace KotaeteMVC.Controllers
{
    public class UserController : AlertControllerBase
    {
        public const string PreviousQuestionKey = "PreviousQuestionKey";
        public const string AjaxFollowersRoute = "UserAjaxFollowers";
        public const string AjaxFollowingRoute = "UserAjaxFollowing";
        private PaginationCreator<ProfileViewModel> _paginationCreator = new PaginationCreator<ProfileViewModel>();


        [Route("user/{userName}", Name = "userProfile")]
        public ActionResult Index(string userName)
        {
            var user = this.GetUserWithName(userName);
            if (user == null)
            {
                return GetUserNotFoundView(userName);
            }
            var currentUser = this.GetCurrentUser();
            ProfileQuestionViewModel userProfile = this.GetProfileQuestionViewModel(userName);
            if (TempData.ContainsKey(PreviousQuestionKey))
            {
                userProfile.QuestionDetail = TempData[PreviousQuestionKey] as QuestionDetailViewModel;
                userProfile.QuestionDetail.AskedToScreenName = user.ScreenName;
                TryValidateModel(userProfile.QuestionDetail);
            }
            return View(userProfile);
        }

        [Route("user/{userName}/following", Name = "userFollowing")]
        [Route("user/{userName}/following/{page}", Name = "userPageFollowing")]
        public ActionResult Following(string userName, int page = 1)
        {
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            var user = this.GetUserWithName(userName);
            if (user == null)
            {
                return GetUserNotFoundView(userName);
            }
            var followerModel = GetFollowingViewModel(user, page);
            if (followerModel.Followers.Count() == 0)
            {
                return View("NoFollowers", new NoFollowersViewModel() { Profile = followerModel.OwnerProfile, IsFollowers = false });
            }
            if (page > followerModel.TotalPages)
            {
                return GetPageNotFoundError();
            }
            return View("Followers", followerModel);
        }

        [Route("user/{userName}/followers", Name = "userFollowers")]
        [Route("user/{userName}/followers/{page}", Name = "userPageFollowers")]
        public ActionResult Followers(string userName, int page = 1)
        {
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            var user = this.GetUserWithName(userName);
            if (user == null)
            {
                return GetUserNotFoundView(userName);
            }
            var followerModel = GetFollowersViewModel(user, page);
            if (followerModel.Followers.Count() == 0)
            {
                return View("NoFollowers", new NoFollowersViewModel() { Profile = followerModel.OwnerProfile, IsFollowers = true });
            }
            if (page > followerModel.TotalPages)
            {
                return GetPageNotFoundError();
            }
            return View("Followers", followerModel);
        }

        [Route("user/{userName}/xhj-followers/{page}", Name = AjaxFollowersRoute)]
        public ActionResult AjaxFollowers(string userName, int page = 1)
        {
            if (Request.IsAjaxRequest() == false)
            {
                return Followers(userName, page);
            }
            if (page < 1)
            {
                return GetPageNotFoundError();
            }
            var user = this.GetUserWithName(userName);
            if (user == null)
            {
                return GetUserNotFoundView(userName);
            }
            var followerModel = GetFollowersViewModel(user, page);
            if (page > followerModel.TotalPages)
            {
                return GetPageNotFoundError();
            }
            return PartialView("MiniProfileGrid", followerModel);
        }

        private ActionResult GetUserNotFoundView(string user)
        {
            var errorModel = new ErrorViewModel()
            {
                ErrorTitle = user + MainGlobal.UserNotFoundErrorHeading,
                ErrorMessage = MainGlobal.UserNotFoundErrorMessage
            };
            return View("Error", errorModel);
        }

        public override int GetPageSize()
        {
            return 9;
        }

        private FollowersViewModel GetFollowersViewModel(ApplicationUser user, int page)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = this.GetProfile(user.UserName);
            var followersQuery = user.Followers.Select(follower => this.GetProfile(follower.UserName));
            var modelInitializer = new PaginationInitializer(AjaxFollowersRoute, "follower-grid", user.UserName, GetPageSize());
            modelInitializer.InitializePaginationModel(followers, page, followersQuery.Count());
            var pageCreator = new PaginationCreator<ProfileViewModel>();
            followers.Followers = pageCreator.GetPage(followersQuery, page, GetPageSize());
            return followers;
        }

        private FollowersViewModel GetFollowingViewModel(ApplicationUser user, int page)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = this.GetProfile(user.UserName);
            var followersQuery = user.Following.Select(follower => this.GetProfile(follower.UserName));
            var modelInitializer = new PaginationInitializer(AjaxFollowingRoute, "follower-grid", user.UserName, GetPageSize());
            modelInitializer.InitializePaginationModel(followers, page, followersQuery.Count());
            var pageCreator = new PaginationCreator<ProfileViewModel>();
            followers.Followers = pageCreator.GetPage(followersQuery, page, GetPageSize());
            return followers;
        }

        [Route("follow")]
        [Authorize]
        public ActionResult FollowUnfollowUser([Bind(Include = "UserToFollowName, Action")] FollowUserViewModel followRequest)
        {
            var userToFollow = this.GetUserWithName(followRequest.UserToFollowName);
            if (userToFollow == null)
            {
                return new HttpUnauthorizedResult("The user " + followRequest.UserToFollowName + " does not exist");
            }
            var current = this.GetCurrentUser();
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
                Context.SaveChanges();
            } catch (Exception e)
            {
                AddAlertDangerOverride("There was an error saving the changes: " + e.Message, "Critical");
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}