using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace KotaeteMVC.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public ApplicationUser()
        {
            Followers = new List<ApplicationUser>();
            Following = new List<ApplicationUser>();
        }

        [ScaffoldColumn(false)]
        [InverseProperty("Following")]
        public List<ApplicationUser> Followers { get; set; }

        [ScaffoldColumn(false)]
        [InverseProperty("Followers")]
        public List<ApplicationUser> Following { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    class TestKotaeteInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };

            manager.Create(adminRole);
            manager.Create(userRole);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var userAdmin = new ApplicationUser { UserName = "Admin" };

            userManager.Create(userAdmin, "ChangeItAsap!");
            userManager.AddToRole(userAdmin.Id, "Admin");

            var user1 = new ApplicationUser { UserName = "User1" };

            userManager.Create(user1, "ChangeItAsap!");
            userManager.AddToRole(user1.Id, "User");

            var user2 = new ApplicationUser { UserName = "User2" };

            userManager.Create(user2, "ChangeItAsap!");
            userManager.AddToRole(user2.Id, "User");

            var user3 = new ApplicationUser { UserName = "User3" };

            userManager.Create(user3, "ChangeItAsap!");
            userManager.AddToRole(user3.Id, "User");

            user1.Following.Add(user2);
            user2.Followers.Add(user1);
            user1.Following.Add(user3);
            user3.Following.Add(user1);
            user3.Followers.Add(user1);
            user1.Followers.Add(user3);

       }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("TestConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new TestKotaeteInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Followers).WithMany(x => x.Following).
                Map(x =>
                {
                    x.ToTable("FollowingFollowerApplicationUsers");
                    x.MapLeftKey("Follower");
                    x.MapRightKey("BeingFollowed");
                });

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}