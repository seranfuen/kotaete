using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KotaeteMVC.Helpers;
using KotaeteMVC.Context;

namespace KotaeteMVC.Controllers
{
    public abstract class BaseController : Controller
    {

        public BaseController() : base()
        {
            Context = new KotaeteDbContext();
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


        public KotaeteDbContext Context { get; private set; }



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