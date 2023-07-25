using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ResignationAPI.Models
{
    public class Resignation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }
        public string? Status { get; set; } 
        public DateTime ResignationDate { get; set; }
        public DateTime RevealingDate { get; set; }
        public string? Reason { get; set; } 
        public string Comments { get; set; } = null!;
        public DateTime CreatedAT { get; set; }
        public DateTime UpdatedAT { get; set;}

        [BsonRepresentation(BsonType.ObjectId)]
        public string? ApprovedBY { get; set; }
    }
}
