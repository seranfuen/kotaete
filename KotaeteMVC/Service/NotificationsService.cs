using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using KotaeteMVC.Helpers;

namespace KotaeteMVC.Service
{
    public class NotificationsService : UsersService
    {
        public NotificationsService(KotaeteDbContext context) : base(context, 0)
        {

        }
        
        public List<Notification> GetLastUserNotifications(string userName, int count, bool onlySeen = false, bool onlyAlerts = false)
        {
            var user = GetUserWithName(userName);
            var subQuery = from notification in _context.Notifications
                           where notification.UserId == user.Id && (!onlySeen || notification.Seen) &&
                           (!onlyAlerts || notification.AllowNotificationAlert)
                           group notification by new { Type = notification.Type, EntityId = notification.EntityId } into g
                           select g;

            // Do another query for questions answered by users being followed

            var query = from g in subQuery.ToList()
                        select (g.OrderByDescending(notification => notification.TimeStamp).FirstOrDefault());

            return query.OrderByDescending(notification => notification.TimeStamp).Take(count).ToList();
        }
            
    }
}