﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KotaeteMVC.Models.Entities
{
    public class QuestionDetail : IEventEntity
    {
        [ScaffoldColumn(false)]
        public virtual int QuestionDetailId { get; set; }

        [ScaffoldColumn(false)]
        public virtual Question Question { get; set; }

        [InverseProperty("QuestionsAsked")]
        [ScaffoldColumn(false)]
        public virtual ApplicationUser AskedBy { get; set; }

        [InverseProperty("QuestionsReceived")]
        [ScaffoldColumn(false)]
        public virtual ApplicationUser AskedTo { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(true)]
        public virtual bool Active { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool SeenByUser { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(false)]
        public virtual bool Answered { get; set; }
    }
}