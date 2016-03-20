using KotaeteMVC.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KotaeteMVC.Models.Entities
{
    public class Answer : IEventEntity
    {

        public Answer()
        {
            Comments = new List<Comment>();
        }

        [ScaffoldColumn(false)]
        public virtual int AnswerId { get; set; }

        public virtual List<AnswerLike> LikesReceived { get; set; }

        [Display(ResourceType = typeof(AnswerStrings), Name = "YourAnswer")]
        [Required]
        [MaxLength(1400)]
        public virtual string Content { get; set; }

        [ScaffoldColumn(false)]
        public virtual ApplicationUser User { get; set; }

        [ScaffoldColumn(false)]
        public virtual int QuestionDetailId { get; set; }

        [ScaffoldColumn(false)]
        public virtual QuestionDetail QuestionDetail { get; set; }

        [ScaffoldColumn(false)]
        public virtual DateTime TimeStamp { get; set; }

        [ScaffoldColumn(false)]
        [DefaultValue(true)]
        public virtual bool Active { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public Comment AddComment(ApplicationUser user, string content)
        {
            var comment = new Comment()
            {
                Answer = this,
                AnswerId = this.AnswerId,
                User = user,
                UserId = user.Id,
                Content = content,
                Active = true,
                TimeStamp = DateTime.Now.AddDays((new Random()).Next(0, 15))
            };
            Comments.Add(comment);
            return comment;
        }
    }
}