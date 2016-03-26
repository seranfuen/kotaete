using KotaeteMVC.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KotaeteMVC.Models.ViewModels.NotificationModels
{
    public class NotificationsListViewModel
    {
        public List<IEventEntity> NotificationEntities { get; set; }
        public ProfileViewModel Profile { get; set; }
    }
}