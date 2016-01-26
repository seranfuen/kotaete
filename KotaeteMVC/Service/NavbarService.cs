using KotaeteMVC.Context;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Service
{
    public class NavbarService : UsersService
    {
        public NavbarService(KotaeteDbContext context) : base(context, 0) { }

        public NavbarViewModel GetNavbarViewModel(bool isAuthenticated)
        {
            var currentUser = GetUserProfile(GetCurrentUserName());
            var model = new NavbarViewModel()
            {
                IsAuthenticated = isAuthenticated,
                InboxCount = GetInboxCount(),
                AvatarUrl = currentUser != null ? currentUser.AvatarUrl : null,
                UserName = currentUser != null ? currentUser.ScreenName : null
            };
            return model;
        }

        public int GetInboxCount()
        {
            var user = GetCurrentUser();
            if (user == null) return 0;
            return _context.QuestionDetails.Count(entity => entity.AskedTo.Id == user.Id && entity.SeenByUser == false);
        }
    }
}