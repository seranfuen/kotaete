using KotaeteMVC.Service;

namespace KotaeteMVC.Controllers
{
    public class NotificationsController : AlertsController
    {
        private NotificationsService _notificationsService;

        public NotificationsController()
        {
            _notificationsService = new NotificationsService(Context);
        }

    }
}