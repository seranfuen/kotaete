﻿using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Helpers;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace KotaeteMVC.Controllers
{
    public class UserController : AlertControllerBase
    {

        public const string PreviousQuestionKey = "PreviousQuestionKey";

        [Route("user/{userName}", Name = "userProfile")]
        [Route("user/{userName}/{request}", Name = "userNameRequest")]
        public ActionResult Index(string userName, string request = "")
        {
            var user = this.GetUserWithName(userName);
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
            var currentUser = this.GetCurrentUser();
            ProfileQuestionViewModel userProfile = this.GetProfileQuestionViewModel(userName);
            if (TempData.ContainsKey(PreviousQuestionKey))
            {
                userProfile.QuestionDetail = TempData[PreviousQuestionKey] as ContentQuestionDetailViewModel;
                userProfile.QuestionDetail.AskedToScreenName = user.ScreenName;
                TryValidateModel(userProfile.QuestionDetail);
            }
            return View(userProfile);
        }

        private FollowersViewModel GetFollowersViewModel(ApplicationUser user)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = this.GetProfileQuestionViewModel(user.UserName);
            followers.Followers = user.Followers.Select(follower => this.GetProfileQuestionViewModel(follower.UserName)).ToList();
            return followers;
        }

        private FollowersViewModel GetFollowingViewModel(ApplicationUser user)
        {
            var followers = new FollowersViewModel();
            followers.OwnerProfile = this.GetProfileQuestionViewModel(user.UserName);
            followers.Followers = user.Following.Select(following => this.GetProfileQuestionViewModel(following.UserName)).ToList();
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