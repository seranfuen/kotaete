using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Models.Entities
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsAsked { get; set; }

        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsReceived { get; set; }

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

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}