using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

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
        public virtual List<ApplicationUser> Followers { get; set; }

        [ScaffoldColumn(false)]
        public virtual List<ApplicationUser> Following { get; set; }

        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsAsked { get; set; }

        [ScaffoldColumn(false)]
        public virtual List<QuestionDetail> QuestionsReceived { get; set; }

        [ScaffoldColumn(false)]
        public virtual string Avatar { get; set; }

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

    class TestKotaeteInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            AddTestUsers(context);
            SetUsersFollowing(context);
        }

        private static void SetUsersFollowing(ApplicationDbContext context)
        {
            var user1 = context.Users.First(user => user.UserName == "user1@kotaete.com");
            var user2 = context.Users.First(user => user.UserName == "user2@kotaete.com");
            var user3 = context.Users.First(user => user.UserName == "user3@kotaete.com");
            user1.ScreenName = "Zakaichou";
            user1.Avatar = "mikan3b.jpg";
            user1.Location = "My harem";
            user1.Bio = "I love lolimoutos!! Give me lolimoutos!!";
            user1.Homepage = @"http://www.twitter.com/lewdhaou";
            user2.ScreenName = "User2";
            user3.ScreenName = "User3";



            user1.Following.Add(user3);
            user1.Following.Add(user2);

            user3.Following.Add(user1);

            context.SaveChanges();
        }

        private static void AddTestUsers(ApplicationDbContext context)
        {
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };

            manager.Create(adminRole);
            manager.Create(userRole);

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var userAdmin = new ApplicationUser { UserName = "admin@kotaete.com", Email = "admin@kotaete.com", ScreenName = "Admin" };

            var user1 = new ApplicationUser { UserName = "user1@kotaete.com", Email = "user1@kotaete.com" };
            var user2 = new ApplicationUser { UserName = "user2@kotaete.com", Email = "user2@kotaete.com", ScreenName = "Kuro von Einzbern" };
            var user3 = new ApplicationUser { UserName = "user3@kotaete.com", Email = "user3@kotaete.com", ScreenName = "Illyasviel von Einzbern"};
            var user4 = new ApplicationUser { UserName = "duck@kotaete.com", Email = "duck@kotaete.com", ScreenName = "Mrs Duck II", Avatar = "DSCF2744.JPG", Location = "Polvoranca", Bio = "I am a duck", Homepage = "http://google.com" };

            userManager.Create(user2, "ChangeItAsap!");
            userManager.AddToRole(user2.Id, "User");

            userManager.Create(user1, "ChangeItAsap!");
            userManager.AddToRole(user1.Id, "User");

            userManager.Create(userAdmin, "N0Rm!1944!");
            userManager.AddToRole(userAdmin.Id, "Admin");

            userManager.Create(user3, "ChangeItAsap!");
            userManager.AddToRole(user3.Id, "User");

            userManager.Create(user4, "ChangeItAsap!");
            userManager.AddToRole(user4.Id, "User");

            context.SaveChanges();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("TestConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new TestKotaeteInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Followers).WithMany(x => x.Following).
                Map(x =>
                {
                    x.ToTable("FollowingFollowerApplicationUsers");
                    x.MapLeftKey("BeingFollowed");
                    x.MapRightKey("Following");
                });

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<KotaeteMVC.Models.Question> Questions { get; set; }

        public DbSet<QuestionDetail> QuestionDetails { get; set; }
    }
}