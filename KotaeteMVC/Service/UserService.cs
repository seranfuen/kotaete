using KotaeteMVC.Context;
using KotaeteMVC.Controllers;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Service
{
    public enum ProfileSaveResult
    {
        OK,
        ImageRejected,
        DuplicateScreenName,
        DatabaseError
    }

    public class UsersService : ServiceBase
    {
        public UsersService(KotaeteDbContext context, int pageSize) : base(context, pageSize)
        {
        }

        public bool ExistsUser(string userName)
        {
            return GetUserWithName(userName) != null;
        }

        public bool FollowUser(string userName)
        {
            var currentUser = GetCurrentUser();
            if (currentUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var userToFollow = GetUserWithName(userName);
            if (IsFollowing(currentUser, userToFollow))
            {
                return false;
            }
            var friendship = new Relationship()
            {
                SourceUser = currentUser,
                DestinationUser = userToFollow,
                RelationshipType = RelationshipType.Friendship,
                Timestamp = DateTime.Now
            };
            _context.Relationships.Add(friendship);
            _context.SaveChanges();
            return true;
        }

        public ApplicationUser GetCurrentUser()
        {
            var currentUserName = GetCurrentUserName();
            return _context.Users.FirstOrDefault(usr => usr.UserName == currentUserName);
        }

        public string GetCurrentUserName()
        {
            return HttpContext.Current.User.Identity.Name;
        }

        public int GetFollowerCount(ApplicationUser user)
        {
            return GetFollowerCount(user.UserName);
        }

        public int GetFollowerCount(string userName)
        {
            return _context.Relationships.Where(rel => rel.RelationshipType == RelationshipType.Friendship).Count(rel =>
                rel.DestinationUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public FollowersViewModel GetFollowersViewModel(string userName)
        {
            var query = GetFollowersQuery(userName);
            FollowersViewModel viewModel = InitializeFollowersViewModel(userName, query);
            return viewModel;
        }

        public List<ApplicationUser> GetFollowers(string userName)
        {
            var query = GetFollowersQuery(userName);
            return query.ToList();
        }

        public FollowersViewModel GetFollowersViewModel(string userName, int page)
        {
            var followersForPage = GetPageFor(GetFollowersQuery(userName), page);
            var followersModel = InitializeFollowersViewModel(userName, followersForPage);
            var paginator = new PaginationInitializer("userPageFollowers", "follower-grid", userName, _pageSize);
            paginator.InitializePaginationModel(followersModel, page, GetFollowerCount(userName));
            return followersModel;
        }

        public int GetFollowingCount(ApplicationUser user)
        {
            return GetFollowingCount(user.UserName);
        }

        public int GetFollowingCount(string userName)
        {
            return _context.Relationships.Where(rel => rel.RelationshipType == RelationshipType.Friendship).Count(rel =>
                rel.SourceUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public FollowersViewModel GetFollowingUsersViewModel(string userName)
        {
            var query = GetFollowingUsersQuery(userName);
            FollowersViewModel viewModel = InitializeFollowersViewModel(userName, query);
            return viewModel;
        }

        public FollowersViewModel GetFollowingUsersViewModel(string userName, int page)
        {
            var followersForPage = GetPageFor(GetFollowingUsersQuery(userName), page);
            var followersModel = InitializeFollowersViewModel(userName, followersForPage);
            var paginator = new PaginationInitializer("userPageFollowing", "follower-grid", userName, _pageSize);
            paginator.InitializePaginationModel(followersModel, page, GetFollowerCount(userName));
            return followersModel;
        }

        public NoFollowersViewModel GetNoFollowersViewModel(string userName)
        {
            return new NoFollowersViewModel() { Profile = GetUserProfile(userName), IsFollowers = true };
        }

        public NoFollowersViewModel GetNotFollowingViewModel(string userName)
        {
            return new NoFollowersViewModel() { Profile = GetUserProfile(userName), IsFollowers = false };
        }

        public ProfileQuestionViewModel GetProfileQuestionViewModel(string userName)
        {
            var userProfile = GetUserProfile(userName);
            var profileQuestion = new ProfileQuestionViewModel()
            {
                Profile = userProfile,
                QuestionDetail = new QuestionDetailViewModel()
                {
                    AskedToScreenName = userProfile.ScreenName,
                    AskedToUserName = userName
                }
            };
            return profileQuestion;
        }

        public ProfileViewModel GetUserProfile(string userName)
        {
            var profileUser = GetUserWithName(userName);
            if (profileUser == null) return null;
            var currentUser = GetCurrentUser();
            return InitializeProfile(currentUser, profileUser);
        }

        public string GetUserScreenName(string userName)
        {
            var user = GetUserWithName(userName);
            return user != null ? user.ScreenName : null;
        }

        public ApplicationUser GetUserWithName(string userName)
       { 
            return _context.Users.FirstOrDefault(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsFollowing(ApplicationUser followingUser, ApplicationUser followedUser)
        {
            if (followingUser == null || followedUser == null)
            {
                return false;
            }
            return _context.Relationships.Where(rel => rel.RelationshipType == RelationshipType.Friendship).Any(rel =>
                rel.SourceUser.Id == followingUser.Id && rel.DestinationUser.Id == followedUser.Id);
        }

        public bool UnfollowUser(string userName)
        {
            var currentUser = GetCurrentUser();
            if (currentUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var userToUnfollow = GetUserWithName(userName);
            if (IsFollowing(currentUser, userToUnfollow) == false)
            {
                return false;
            }
            _context.Relationships.Remove(_context.Relationships.First(rel => rel.RelationshipType == RelationshipType.Friendship &&
              rel.SourceUser.Id == currentUser.Id && rel.DestinationUser.Id == userToUnfollow.Id));
            _context.SaveChanges();
            return true;
        }

        public string GetAvatarUrl(ApplicationUser user)
        {
            if (user == null) return null;
            var url = "/Images/Avatars/";
            if (user.Avatar != null)
            {
                return url + user.Avatar;
            }
            return url + "anonymous.jpg";
        }

        private IQueryable<ApplicationUser> GetFollowersQuery(string sourceUserName)
        {
            var query = from relationship in _context.Relationships
                        where relationship.DestinationUser.UserName.Equals(sourceUserName, StringComparison.OrdinalIgnoreCase)
                        orderby relationship.Timestamp descending
                        select relationship.SourceUser;
            return query;
        }

        private IQueryable<ApplicationUser> GetFollowingUsersQuery(string userName)
        {
            var query = from relationship in _context.Relationships
                        where relationship.SourceUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                        orderby relationship.Timestamp descending
                        select relationship.DestinationUser;
            return query;
        }

        public string GetHeaderUrl(ApplicationUser user)
        {
            var url = "/Images/Headers/";
            if (user.Header != null)
            {
                return url + user.Header;
            }
            return null;
        }

        private FollowersViewModel InitializeFollowersViewModel(string destinationUserName, IEnumerable<ApplicationUser> query)
        {
            return new FollowersViewModel()
            {
                Followers = query.ToList().Select(user => GetUserProfile(user.UserName)).ToList(),
                OwnerProfile = GetUserProfile(destinationUserName)
            };
        }

        public FollowButtonViewModel GetFollowButtonViewModel(string userName)
        {
            var currentUser = GetCurrentUser();
            var followingUser = GetUserWithName(userName);
            var isFollowing = IsFollowing(currentUser, followingUser);
            return GetFollowButtonViewModel(userName, isFollowing, currentUser != null);
        }

        private FollowButtonViewModel GetFollowButtonViewModel(string userName, bool isFollowing, bool isAuthenticated)
        {
            var screenName = this.GetUserScreenName(userName);
            return new FollowButtonViewModel()
            {
                UserName = userName,
                IsFollowing = isFollowing,
                IsUserAuthenticated = isAuthenticated,
                SuccessFollowMessage = isFollowing ? AlertsController.UnfollowSuccessKey : AlertsController.FollowSuccessKey,
                FailureMessage = UsersStrings.FollowingError,
                IsOwnProfile = userName.Equals(GetCurrentUserName(), StringComparison.OrdinalIgnoreCase)
            };
        }

        private ProfileViewModel InitializeProfile(ApplicationUser currentUser, ApplicationUser profileUser)
        {
            var questionService = new QuestionsService(_context, _pageSize);
            var isCurrentUserFollowing = IsFollowing(currentUser, profileUser);
            return new ProfileViewModel()
            {
                ScreenName = profileUser.ScreenName,
                FollowsYou = IsFollowing(profileUser, currentUser),
                Following = IsFollowing(currentUser, profileUser),
                IsOwnProfile = currentUser != null && currentUser.UserName == profileUser.UserName,
                AvatarUrl = GetAvatarUrl(profileUser),
                HeaderUrl = GetHeaderUrl(profileUser),
                Bio = profileUser.Bio,
                Location = profileUser.Location,
                Homepage = profileUser.Homepage,
                User = profileUser,
                QuestionsReplied = questionService.GetQuestionsAnsweredByUser(profileUser),
                QuestionsAsked = questionService.GetQuestionsAskedByUser(profileUser),
                FollowerCount = GetFollowerCount(profileUser),
                FollowingCount = GetFollowingCount(profileUser),
                FollowButton = GetFollowButtonViewModel(profileUser.UserName, isCurrentUserFollowing, currentUser != null),
                AnswerLikesCount = _context.AnswerLikes.Count(like => like.Active && like.ApplicationUserId == profileUser.Id)
            };
        }

        public ProfileSaveResult SaveProfile(ApplicationUser userModel)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return ProfileSaveResult.DatabaseError;
            }
            if (_context.Users.Any(user => user.Id != currentUser.Id && user.ScreenName.Equals(userModel.ScreenName, StringComparison.OrdinalIgnoreCase)))
            {
                return ProfileSaveResult.DuplicateScreenName;
            }
            currentUser.ScreenName = userModel.ScreenName;
            if (string.IsNullOrWhiteSpace(userModel.Avatar) == false)
            {
                var image = ExtractImage(userModel.Avatar);
                if (image != null)
                {
                    try
                    {
                        currentUser.Avatar = SaveImage(currentUser, image, "Avatars");
                    }
                    catch (Exception e)
                    {
                        return ProfileSaveResult.ImageRejected;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(userModel.Header) == false)
            {
                var image = ExtractImage(userModel.Header);
                try
                {
                    currentUser.Header = SaveImage(currentUser, image, "Headers");
                }
                catch (Exception e)
                {
                    return ProfileSaveResult.ImageRejected;
                }
            }
            currentUser.Bio = userModel.Bio;
            currentUser.Location = userModel.Location;
            currentUser.Homepage = userModel.Homepage;
            try
            {
                _context.SaveChanges();
                return ProfileSaveResult.OK;
            } catch (Exception e)
            {
                return ProfileSaveResult.DatabaseError;
            }
            
        }

        private static string SaveImage(ApplicationUser currentUser, Image image, string imageFolder)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            path = Path.Combine(path, imageFolder);
            var fileName = currentUser.UserName + "-" + Guid.NewGuid().ToString() + ".jpg";
            path = Path.Combine(path, fileName);
            image.Save(path, ImageFormat.Jpeg);
            return fileName;
        }

        private Image ExtractImage(string base64Image)
        {
            if (base64Image.IndexOf(",") == -1)
            {
                return null;
            }
            var mime = base64Image.Substring(0, base64Image.IndexOf(","));
            var imageString = base64Image.Substring(base64Image.IndexOf(",") + 1);
            byte[] imageBytes = Convert.FromBase64String(imageString);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                try {
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
                catch (ArgumentException e)
                {
                    return null;
                }
            }
        }
    }
}