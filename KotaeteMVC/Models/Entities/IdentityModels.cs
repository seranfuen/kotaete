using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KotaeteMVC.Models.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsAsked { get; set; }

        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsReceived { get; set; }

        public virtual List<AnswerLike> AnswerLikes { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Avatar { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Header { get; set; }

        [ScaffoldColumn(false)]
        public virtual string ScreenName { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Location { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Bio { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Homepage { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}