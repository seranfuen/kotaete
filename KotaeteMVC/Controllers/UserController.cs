using KotaeteMVC.Helpers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using KotaeteMVC.Service;
using Resources;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class UserController : AlertsController
    {
        public const string PreviousQuestionKey = "PreviousQuestionKey";
        private PaginationCreator<ProfileViewModel> _paginationCreator = new PaginationCreator<ProfileViewModel>();
        private UsersService _usersService;

        public UserController()
        {
            _usersService = new UsersService(Context, GetPageSize());
        }

        [Route("user/{userName}/followers", Name = "userFollowers")]
        [Route("user/{userName}/followers/{page}", Name = "userPageFollowers")]
        public ActionResult Followers(string userName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (_usersService.ExistsUser(userName) == false)
            {
                return GetUserNotFoundView(userName);
            }
            else if (_usersService.GetFollowerCount(userName) == 0)
            {
                return View("NoFollowers", _usersService.GetNoFollowersViewModel(userName));
            }
            var followerModel = _usersService.GetFollowersViewModel(userName, page);
            if (Request.IsAjaxRequest())
            {
                return PartialView("MiniProfileGrid", followerModel);
            }
            else
            {
                ViewBag.HeaderImage = followerModel.OwnerProfile.HeaderUrl;
                return View("Followers", followerModel);
            }
        }

        [Route("user/{userName}/following", Name = "userFollowing")]
        [Route("user/{userName}/following/{page}", Name = "userPageFollowing")]
        public ActionResult Following(string userName, int page = 1)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (_usersService.ExistsUser(userName) == false)
            {
                return GetUserNotFoundView(userName);
            }
            else if (_usersService.GetFollowingCount(userName) == 0)
            {
                return View("NoFollowers", _usersService.GetNotFollowingViewModel(userName));
            }
            var followerModel = _usersService.GetFollowingUsersViewModel(userName, page);
            if (Request.IsAjaxRequest())
            {
                return PartialView("MiniProfileGrid", followerModel);
            }
            else
            {
                ViewBag.HeaderImage = followerModel.OwnerProfile.HeaderUrl;
                return View("Following", followerModel);
            }
        }

        [Authorize]
        [Route("user/{userName}/follow")]
        public ActionResult FollowUser(string userName)
        {
            var result = _usersService.FollowUser(userName);
            if (result)
            {
                if (Request.IsAjaxRequest())
                {
                    var model = _usersService.GetFollowButtonViewModel(userName);
                    return PartialView("FollowButton", model);
                }
                else
                {
                    AddAlertSuccess(UsersStrings.FollowingSuccess + _usersService.GetUserScreenName(userName), "");
                    return RedirectToPrevious();
                }
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
        }

        public override int GetPageSize()
        {
            return 9;
        }

        [Route("user/{userName}", Name = "userProfile")]
        public ActionResult Index(string userName)
        {
            if (_usersService.ExistsUser(userName) == false)
            {
                return GetUserNotFoundView(userName);
            }
            var userProfileModel = _usersService.GetProfileQuestionViewModel(userName);
            ViewBag.HeaderImage = userProfileModel.Profile.HeaderUrl;
            return View(userProfileModel);
        }

        public PartialViewResult UserProfile(string userName)
        {
            if (_usersService.ExistsUser(userName) == false)
            {
                return null;
            }
            var profile = _usersService.GetUserProfile(userName);
            return PartialView("Profile", profile);
        }

        [Authorize]
        [Route("user/{userName}/unfollow")]
        public ActionResult UnfollowUser(string userName)
        {
            var result = _usersService.UnfollowUser(userName);
            if (result)
            {
                if (Request.IsAjaxRequest())
                {
                    var model = _usersService.GetFollowButtonViewModel(userName);
                    return PartialView("FollowButton", model);
                }
                else
                {
                    AddAlertSuccess(UsersStrings.UnfollowingSuccessFst + _usersService.GetUserScreenName(userName) + UsersStrings.UnfollowingSuccessLst, "");
                    return RedirectToPrevious();
                }
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
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

        private void InitializePagination(FollowersViewModel followerModel, string route, string userName, int page)
        {
            var paginator = new PaginationInitializer("userPageFollowers", "follower-grid", userName, GetPageSize());
            paginator.InitializePaginationModel(followerModel, page, _usersService.GetFollowingCount(userName));
        }
    }
}