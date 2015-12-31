using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    /// <summary>
    /// View Model that binds a user id that gets a question asked and a question content
    /// </summary>
    public class ProfileQuestionViewModel
    {

        public virtual ApplicationUser User { get; set; }

        public virtual string ProfileUserName { get; set; }

        public virtual string QuestionContent { get; set; }

        public virtual bool FollowsYou { get; set; }

        public virtual bool Following { get; set; }

        public virtual bool IsOwnProfile { get; set; }

        public virtual bool CurrentUserAuthenticated { get; set; }

        public virtual string AvatarUrl { get; set; }

        public virtual string HeaderUrl { get; set; }

        public virtual string Location { get; set; }

        public virtual string Bio { get; set; }

        public virtual string Homepage { get; set; }

        public virtual int QuestionsAsked { get; set; }

        public virtual int QuestionsReplied { get; set; }

        [DefaultValue(0)]
        public virtual int FollowingCount { get; set; }

        [DefaultValue(0)]
        public virtual int FollowerCount { get; set; }
    }
}