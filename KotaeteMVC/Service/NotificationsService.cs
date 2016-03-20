using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using KotaeteMVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Service
{
    public class NotificationsService : UsersService
    {
        public NotificationsService(KotaeteDbContext context) : base(context, 0)
        {

        }

        public List<IEventEntity> GetLastEventEntities(string userName, int count)
        {
            throw new NotImplementedException();
        }

        //public List<FollowerNotificationViewModel> GetLastFollowersNotification(string userName, int number = 10)
        //{
        //    var relationships = GetLastFollowers(userName, number);
        //    var query = from relationship in relationships
        //                select InitializeModel(relationship);
        //    return query.ToList();
        //}

        //public List<FollowerNotificationViewModel> GetLastFollowingNotification(string userName, int number = 10)
        //{
        //    var relationships = GetLastFollowing(userName, number);
        //    var query = from relationship in relationships
        //                select InitializeModel(relationship);
        //    return query.ToList();
        //}

        //private FollowerNotificationViewModel InitializeModel(Relationship relationship)
        //{
        //    var model = new FollowerNotificationViewModel()
        //    {
        //        FollowedUser = GetUserProfile(relationship.DestinationUser.UserName),
        //        FollowingUser = GetUserProfile(relationship.SourceUser.UserName),
        //        Timestamp = relationship.TimeStamp
        //    };
        //    return model;
        //}

        //private List<Relationship> GetLastFollowers(string userName, int number = 10)
        //{
        //    return _context.Relationships.Where(rel => rel.DestinationUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && rel.Active &&
        //        rel.RelationshipType == RelationshipType.Friendship).OrderByDescending(rel => rel.TimeStamp).Take(number).ToList();
        //}

        //private List<Relationship> GetLastFollowing(string userName, int number = 10)
        //{
        //    return _context.Relationships.Where(rel => rel.SourceUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && rel.Active &&
        //        rel.RelationshipType == RelationshipType.Friendship).OrderByDescending(rel => rel.TimeStamp).Take(number).ToList();
        //}

    }
}