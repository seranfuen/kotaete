using KotaeteMVC.Context;
using KotaeteMVC.Models;
using Resources;
using System.Net;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController() : base()
        {
            Context = new KotaeteDbContext();
        }

        public ActionResult RedirectToPrevious()
        {
            if (Request.UrlReferrer == null)
            {
                return GetDefaultView();
            }
            else
            {
                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public virtual ActionResult GetDefaultView()
        {
            return RedirectToAction("Index", "Home");
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

        public HttpStatusCodeResult GetBadRequestResult()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        public HttpStatusCodeResult GetAcceptedResult()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Accepted);
        }
    }
}