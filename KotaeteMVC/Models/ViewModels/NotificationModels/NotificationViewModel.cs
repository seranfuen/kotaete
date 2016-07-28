namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class NotificationViewModel
    {
        public NotificationViewModel(object entity, bool seen)
        {
            Entity = entity;
            Seen = seen;
        }

        public object Entity { get; set; }
        public bool Seen { get; set; }
    }
}