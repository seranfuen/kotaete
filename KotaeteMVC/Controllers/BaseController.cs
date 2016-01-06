using KotaeteMVC.Models;
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

        public ApplicationDbContext Context { get; private set; }

        public BaseController()
        {
            Context = new ApplicationDbContext();
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