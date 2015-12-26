using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models
{
    /// <summary>
    /// View Model that binds a user id that gets a question asked and a question content
    /// </summary>
    public class ProfileQuestionViewModel
    {
        public virtual string AskedUserName { get; set; }

        public virtual string QuestionContent { get; set; }
    }
}