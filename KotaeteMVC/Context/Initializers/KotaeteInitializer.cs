using KotaeteMVC.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;

namespace KotaeteMVC.Context.Initializers
{
    class KotaeteInitializer : DropCreateDatabaseAlways<KotaeteDbContext>
    {
        private KotaeteDbContext _context;

        protected override void Seed(KotaeteDbContext context)
        {
            _context = context;
            AddTestUsers();
            AddFollowingTestUsers();
            AddQuestionsAnswers();
            _context.SaveChanges();
        }

        private void AddFollowingTestUsers()
        {
            var followed = CreateApplicationUser("Followers", "Followers");
            AddUser(followed);
            for (int i = 0; i < 12; i++)
            {
                var nUser = CreateApplicationUser("User" + i, "User" + i);
                _context.Relationships.Add(new Relationship()
                {
                    DestinationUser = followed,
                    SourceUser = nUser,
                    RelationshipType = RelationshipType.Friendship,
                    Timestamp = DateTime.Now
                });
                AddUser(nUser);
            }
        }

        private void AddQuestionsAnswers()
        {
            for (int i = 0; i < 35; i++)
            {
                var answer = new Answer()
                {
                    Content = "TESTTESTTEST " + i.ToString(),
                    Deleted = false,
                    TimeStamp = DateTime.Now.AddDays(-i).AddHours(i),
                    User = _context.Users.First(user => user.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase)),
                    QuestionDetail = new QuestionDetail()
                    {
                        AskedBy = _context.Users.First(user => user.UserName.Equals("turtle", StringComparison.OrdinalIgnoreCase)),
                        AskedTo = _context.Users.First(user => user.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase)),
                        Answered = true,
                        SeenByUser = true,
                        TimeStamp = DateTime.Now.AddDays(-i),
                        Deleted = false,
                        Question = new Question()
                        {
                            AskedBy = _context.Users.First(user => user.UserName.Equals("turtle", StringComparison.OrdinalIgnoreCase)),
                            TimeStamp = DateTime.Now.AddDays(-i),
                            Content = "QUESTION QUESTION QUESTION " + i.ToString()
                        }
                    }
                };
                _context.Answers.Add(answer);
            }
        }

        private ApplicationUser CreateApplicationUser(string userName, string screenName, string avatar = null, string header = null)
        {
            var user = new ApplicationUser() { UserName = userName, Email = userName + "@kotaete.com", ScreenName = screenName, Avatar = avatar, Header = header };
            return user;
        }

        private void AddUser(ApplicationUser user, string role = "User")
        {
            var userStore = new UserStore<ApplicationUser>(_context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.Create(user, "norm1944");
            userManager.AddToRole(user.Id, role);
        }

        private void AddTestUsers()
        {
            InitializeRoles();
            var admin = CreateApplicationUser("Admin", "Administrator");
            AddUser(admin, "admin");

            var duck = CreateApplicationUser("Duck", "Mr Duck", "DSCF2744.JPG", "DSCF2412.jpg");
            duck.Location = "Duckland";
            duck.Bio = "I am a duck";
            duck.Homepage = @"http://google.com";
            AddUser(duck);

            var turtle = CreateApplicationUser("Turtle", "Mrs Turtle", "DSCF1904.jpg", "DSCF1809.jpg");
            AddUser(turtle);

            SetFollowing(duck, turtle);
            SetFollowing(duck, admin);
            SetFollowing(admin, duck);
            SetFollowing(turtle, admin);

            AskQuestion(turtle, admin, "Do you like Turtles?", DateTime.Now.AddDays(-1).AddHours(-5), true);
            AskQuestion(duck, admin, "Do you like ducks?", DateTime.Now.AddMinutes(-5), false);
        }

        private void AskQuestion(ApplicationUser askingUser, ApplicationUser askedUser, string question, DateTime time, bool seen = false)
        {
            var qst = new Question()
            {
                AskedBy = askingUser,
                Content = question,
                TimeStamp = time
            };
            var qstDetail = new QuestionDetail()
            {
                AskedBy = askingUser,
                AskedTo = askedUser,
                Deleted = false,
                SeenByUser = seen,
                Question = qst,
                TimeStamp = qst.TimeStamp
            };
            _context.Questions.Add(qst);
            _context.QuestionDetails.Add(qstDetail);
        }

        private void SetFollowing(ApplicationUser followingUser, params ApplicationUser[] followedUser)
        {
            var relationships = followedUser.Select(followed => new Relationship()
            {
                DestinationUser = followed,
                SourceUser = followingUser,
                RelationshipType = RelationshipType.Friendship,
                Timestamp = DateTime.Now
            });
            _context.Relationships.AddRange(relationships);
        }

        private void InitializeRoles()
        {
            var store = new RoleStore<IdentityRole>(_context);
            var manager = new RoleManager<IdentityRole>(store);
            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };

            manager.Create(adminRole);
            manager.Create(userRole);
        }
    }
}