using KotaeteMVC.Service;
using System.Web.Mvc;

namespace KotaeteMVC.Controllers
{
    public class NavbarController : BaseController
    {
        private NavbarService _navbarService;

        public NavbarController()
        {
            _navbarService = new NavbarService(Context);
        }

        public ActionResult Index()
        {
            var model = _navbarService.GetNavbarViewModel(Request.IsAuthenticated);
            return PartialView("Index", model);
        }
    }
}