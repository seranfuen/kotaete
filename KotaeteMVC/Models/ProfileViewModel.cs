using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    public class ProfileViewModel
    {
        public bool FollowsYou { get; set; }

        public bool Following { get; set; }

        public ApplicationUser User { get; set; }

        public string ScreenName { get; set; }

        public bool IsOwnProfile { get; set; }

        public string AvatarUrl { get; set; }

        public string HeaderUrl { get; set; }

        public string Location { get; set; }

        public string Bio { get; set; }

        public string Homepage { get; set; }

        public int QuestionsAsked { get; set; }

        public int QuestionsReplied { get; set; }

        [DefaultValue(0)]
        public int FollowingCount { get; set; }

        [DefaultValue(0)]
        public int FollowerCount { get; set; }
    }
}