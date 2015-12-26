using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace KotaeteMVC.Controllers
{
    public class UserController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult Index(string userName, string request = "")
        {
            var user = GetUserWithName(userName);
            if (user == null)
            {
                ViewBag.UserName = userName;
                return View("UserNotFound");
            }
            if (request == "Followers")
            {
                return View("Followers", user);
            } else if (request == "Following")
            {
                return View("Following", user);
            }
            var userProfile = new ProfileQuestionViewModel()
            {
                AskedUserName = userName
            };
            return View(userProfile);
        }

        private ApplicationUser GetUserWithName(string userName)
        {
            return db.Users.FirstOrDefault(usr => usr.UserName == userName);
        }

        [Authorize]
        public ActionResult AskQuestion([Bind(Include = "AskedUserName, QuestionContent")] ProfileQuestionViewModel question)
        {
            var askedUser = GetUserWithName(question.AskedUserName);
            ApplicationUser asker = db.Users.FirstOrDefault(usr => usr.UserName == HttpContext.User.Identity.Name);

            var now = DateTime.Now;

            var qstDetail = new QuestionDetail()
            {
                Question = new Question()
                {
                    Content = question.QuestionContent,
                    TimeStamp = now,
                    AskedBy = asker,
                },
                AskedTo = askedUser,
                AskedBy = asker,
                TimeStamp = now
            };

            db.QuestionDetails.Add(qstDetail);

            db.SaveChanges();
            return Index(question.AskedUserName);
        }

        
    }
}