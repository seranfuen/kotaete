using KotaeteMVC.Models;
using KotaeteMVC.Models.Entities;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public abstract class BaseController : Controller
    {

        public virtual int GetPageSize()
        {
            return 5;
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

        public BaseController()
        {
            Context = new ApplicationDbContext();
        }

        public int GetPageCount(int itemCount)
        {
            var pages = itemCount / GetPageSize();
            if (itemCount % GetPageSize() > 0)
            {
                return pages + 1;
            }
            return pages;
        }

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