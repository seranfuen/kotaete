using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Models.Entities
{
    public enum RelationshipType
    {
        Friendship,
        Block
    }

    public class Relationship
    {
        public virtual int RelationshipId { get; set; }
        public virtual DateTime Timestamp { get; set; }
        [Required]
        [ForeignKey("DestinationUserId")]
        public virtual ApplicationUser SourceUser { get; set; }
        [Required]
        [ForeignKey("SourceUserId")]
        public virtual ApplicationUser DestinationUser { get; set; }


        public virtual string DestinationUserId { get; set; }
        public virtual string SourceUserId { get; set; }

        [Required]
        public virtual RelationshipType RelationshipType { get; set; }
    }
}
