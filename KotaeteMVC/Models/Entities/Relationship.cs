using System;
using System.Collections.Generic;
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
        public DateTime Timestamp { get; set; }
        public ApplicationUser SourceUser { get; set; }
        public ApplicationUser DestinationUser { get; set; }
        public RelationshipType RelationshipType { get; set; }
    }
}