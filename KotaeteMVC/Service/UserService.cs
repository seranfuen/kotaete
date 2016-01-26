using KotaeteMVC.Context;
using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using KotaeteMVC.Models.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Service
{
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
                rel.SourceUser == followingUser && rel.DestinationUser == followedUser);
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
                        select relationship.SourceUser;
            return query;
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

        private FollowersViewModel InitializeFollowersViewModel(string destinationUserName, IEnumerable<ApplicationUser> query)
        {
            return new FollowersViewModel()
            {
                Followers = query.ToList().Select(user => GetUserProfile(user.UserName)).ToList(),
                OwnerProfile = GetUserProfile(destinationUserName)
            };
        }

        private ProfileViewModel InitializeProfile(ApplicationUser currentUser, ApplicationUser profileUser)
        {
            var questionService = new QuestionsService(_context, _pageSize);
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
                FollowButton = new FollowButtonViewModel()
                {
                    UserName = profileUser.UserName,
                    IsFollowing = IsFollowing(profileUser, currentUser)
                }
            };
        }
    }
}