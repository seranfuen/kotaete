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
        
        public List<Notification> GetLastUserNotifications(string userName, int count, bool onlySeen = false, bool onlyAlerts = false, bool showFriendEvents = false)
        {
            var user = GetUserWithName(userName);
            var subFollowing = _context.Relationships.Where(rel => rel.Active && rel.SourceUserId == user.Id);
            var following = subFollowing.Select(rel => rel.DestinationUserId).ToList();

            var subQuery = from notification in _context.Notifications
                           where notification.UserId == user.Id && 
                            (!onlySeen || notification.Seen) &&
                            (!onlyAlerts || notification.AllowNotificationAlert)
                           group notification by new { Type = notification.Type, EntityId = notification.EntityId } into g
                           select g;

            var query = from g in subQuery.ToList()
                        select (g.OrderByDescending(notification => notification.TimeStamp).FirstOrDefault());

            var notifications = query.OrderByDescending(notification => notification.TimeStamp).Take(count).ToList();

            notifications.AddRange(GetFriendRelationshipNotifications(following, notifications));
            notifications.AddRange(GetFriendAnsweredNotifications(following, notifications));

            return notifications.OrderByDescending(not => not.TimeStamp).Take(count).ToList();
        }

        private List<Notification> GetFriendRelationshipNotifications(List<string> followingUserIds, List<Notification> userNotificationsToFilter)
        {
            var list = GetFriendRelationshipNotifications(followingUserIds);
            return FilterList(list, userNotificationsToFilter);
        }

        private List<Notification> FilterList(List<Notification> list, List<Notification> userNotificationsToFilter)
        {
            var query =
                from notification in list
                where !userNotificationsToFilter.Any(userNot => userNot.Type == notification.Type && userNot.EntityId == notification.EntityId)
                select notification;
            return query.ToList();
        }

        private List<Notification> GetFriendRelationshipNotifications(List<string> followingUserIds)
        {
            var friendsRelSubQuery =
              from notification in _context.Notifications
              where followingUserIds.Contains(notification.UserId) &&
                    notification.Type == Notification.NotificationType.Relationship
              group notification by new { Type = notification.Type, EntityId = notification.EntityId } into g
              select g;

            var friendsRelQuery = from g in friendsRelSubQuery.ToList()
                               select (g.OrderByDescending(notification => notification.TimeStamp).FirstOrDefault());
            return friendsRelQuery.ToList();
        }

        private List<Notification> GetFriendAnsweredNotifications(List<string> followingUserIds, List<Notification> userNotificationsToFilter)
        {
            var list = GetFriendAnsweredNotifications(followingUserIds);
            return FilterList(list, userNotificationsToFilter);
        }

        private List<Notification> GetFriendAnsweredNotifications(List<string> followingUserIds)
        {
            var friendsAnswersSubQuery =
                from notification in _context.Notifications
                join answer in _context.Answers on notification.EntityId equals answer.AnswerId
                where notification.Type == Notification.NotificationType.Answer &&
                      answer.Active &&
                      followingUserIds.Contains(answer.User.Id)
                group notification by notification.EntityId into g
                select g;

            var friendsAnswersQuery = from g in friendsAnswersSubQuery.ToList()
                                  select (g.OrderByDescending(notification => notification.TimeStamp).FirstOrDefault());
            return friendsAnswersQuery.ToList();
        }
    }
}