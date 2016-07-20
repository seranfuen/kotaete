using KotaeteMVC.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using KotaeteMVC.Service;

namespace KotaeteMVC.Context.Initializers
{
    internal class KotaeteInitializer : DropCreateDatabaseAlways<KotaeteDbContext>
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
            var userService = new UsersService(_context, 10);
            for (int i = 0; i < 12; i++)
            {
                var nUser = CreateApplicationUser("User" + i, "User" + i);
                AddUser(nUser);
                userService.FollowUser(followed.UserName, nUser.UserName);
            }
        }

        private void AddQuestionsAnswers()
        {
            var questionsService = new QuestionsService(_context, 10);
            var answersService = new AnswersService(_context, 10);
            for (int i = 0; i < 3; i++)
            {
                var detail = questionsService.SaveQuestionDetail("admin", "turtle", "This is a test question " + i.ToString());
                detail.TimeStamp.AddDays(-i);

                var answer = answersService.SaveAnswer("admin", "This is a test answer to question #" + i.ToString(), detail.QuestionDetailId);
                answer.TimeStamp = answer.TimeStamp.AddDays(-i).AddMinutes(35);
                AddRandomComments(answer);
            }
        }

        private void AddRandomComments(Answer answer)
        {
            var rnd = new Random();
            var comments = rnd.Next(6);
            for (int i = 0; i < comments; i++)
            {
                answer.AddComment(answer.User, "COMMENT\r\n" + i.ToString());
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
            var validator = (MinimumLengthValidator)userManager.PasswordValidator;
            validator.RequiredLength = 1;
            var result = userManager.Create(user, "1");
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
            var questionsService = new QuestionsService(_context, 10);
            var qstDetail = questionsService.SaveQuestionDetail(askedUser.UserName, askingUser.UserName, question);
            qstDetail.TimeStamp = time;
            qstDetail.SeenByUser = seen;
            qstDetail.Question.TimeStamp = time;
        }

        private void SetFollowing(ApplicationUser followingUser, params ApplicationUser[] followedUsers)
        {
            var usersService = new UsersService(_context, 10);
            followedUsers.ToList().ForEach(user => usersService.FollowUser(user.UserName, followingUser.UserName));
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