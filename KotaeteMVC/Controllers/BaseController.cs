using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Helpers;

namespace KotaeteMVC.Controllers
{
    public abstract class BaseController : Controller
    {

        public BaseController() : base()
        {
            Context = new ApplicationDbContext();
        }

        protected int GetInboxCount()
        {
            var user = this.GetCurrentUser();
            if (user == null) return 0;
            return Context.QuestionDetails.Count(entity => entity.AskedTo.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase) &&
                entity.SeenByUser == false);
        }

        public virtual int GetPageSize()
        {
            return 5;
        }

        protected ActionResult GetPageNotFoundError()
        {
            return GetErrorView(MainGlobal.PageNotFoundErrorHeader, MainGlobal.PageNotFoundErrorMessage);
        }


        public ActionResult GetErrorView(string header, string message)
        {
            var errorModel = new ErrorViewModel()
            {
                ErrorTitle = header,
                ErrorMessage = message
            };
            return View("Error", errorModel);
        }


        public ApplicationDbContext Context { get; private set; }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}