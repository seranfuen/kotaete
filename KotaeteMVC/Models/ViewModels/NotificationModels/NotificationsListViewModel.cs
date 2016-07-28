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
        public NotificationsListViewModel(Dictionary<IEventEntity, bool> dictionaryEntitiesSeen, ProfileViewModel profile)
        {
            NotificationEntities = dictionaryEntitiesSeen.Keys.ToList();
            SeenDictionary = dictionaryEntitiesSeen;
            Profile = profile;
        }

        public List<IEventEntity> NotificationEntities { get; private set; }
        public ProfileViewModel Profile { get; private set; }
        public Dictionary<IEventEntity, bool> SeenDictionary { get; private set; }
    }
}