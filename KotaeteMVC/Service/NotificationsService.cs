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
            var queryRels = GetRelationshipEventsForUser(userName);
            var queryQuestions = GetQuestionDetailEventsForUser(userName);
            var queryAnswers = GetAnswerEventsForUser(userName);
            var queryLikes = GetAnswerLikesForUser(userName);
            var queryComments = GetCommentsForUser(userName);
            var queryUnion = queryRels.Union(queryQuestions).Union(queryAnswers).Union(queryLikes).Union(queryComments);
            var query = from notification in queryUnion
                        orderby notification.TimeStamp descending
                        select notification;
            return query.Take(count).ToList();
        }

        private List<IEventEntity> GetRelationshipEventsForUser(string userName)
        {
            var queryRelationships = from rel in _context.Relationships
                                     where rel.Active &&
                                     (rel.DestinationUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) || rel.SourceUser.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                                     select rel;
            return queryRelationships.ToList().Cast<IEventEntity>().ToList();
        }

        private List<IEventEntity> GetQuestionDetailEventsForUser(string userName)
        {
            var queryQuestions = from question in _context.QuestionDetails
                                     where !question.Answered && question.AskedTo.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && question.Active
                                     select question;
            return queryQuestions.ToList().Cast<IEventEntity>().ToList();
        }

        private List<IEventEntity> GetAnswerEventsForUser(string userName)
        {
            var queryAnswers = from answer in _context.Answers
                               where answer.Active && answer.QuestionDetail.AskedBy.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                               select answer;
            return queryAnswers.ToList().Cast<IEventEntity>().ToList();
        }

        private List<IEventEntity> GetAnswerLikesForUser(string userName)
        {
            var queryLikes = from answerLike in _context.AnswerLikes
                             where answerLike.Active && (answerLike.Answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                             select answerLike;
            return queryLikes.ToList().Cast<IEventEntity>().ToList();
        }

        private List<IEventEntity> GetCommentsForUser(string userName)
        {
            var queryComments = from comment in _context.Comments
                                where comment.Active && (comment.Answer.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) ||
                                comment.Answer.Comments.Any(otherComment => otherComment.User.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
                                select comment;
            return queryComments.ToList().Cast<IEventEntity>().ToList();

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