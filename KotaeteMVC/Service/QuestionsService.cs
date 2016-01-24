using KotaeteMVC.Context;
using KotaeteMVC.Models.Entities;
using System.Linq;

namespace KotaeteMVC.Service
{
    public class QuestionsService : ServiceBase
    {
        public QuestionsService(KotaeteDbContext context) : base(context)
        {
        }

        public int GetQuestionsAnsweredByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Deleted == false && answer.User == user);
        }

        public int GetQuestionsAskedByUser(ApplicationUser user)
        {
            return _context.Answers.Count(answer => answer.Deleted == false && answer.QuestionDetail.AskedBy == user);
        }
    }
}