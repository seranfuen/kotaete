using KotaeteMVC.Context.Initializers;
using KotaeteMVC.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace KotaeteMVC.Context
{
    public class KotaeteDbContext : IdentityDbContext<ApplicationUser>
    {
        public KotaeteDbContext()
            : base("TestConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new KotaeteInitializer());
        }

        public static KotaeteDbContext Create()
        {
            return new KotaeteDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Relationship>()
            .HasRequired(c => c.DestinationUser)
            .WithMany()
            .WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Question> Questions { get; set; }

        public DbSet<QuestionDetail> QuestionDetails { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Relationship> Relationships { get; set; }

        public DbSet<AnswerLike> AnswerLikes { get; set; }
    }
}