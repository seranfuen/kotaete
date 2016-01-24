using KotaeteMVC.Context.Initializers;
using KotaeteMVC.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

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

        public DbSet<Question> Questions { get; set; }

        public DbSet<QuestionDetail> QuestionDetails { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Relationship> Relationships { get; set; }
    }
}