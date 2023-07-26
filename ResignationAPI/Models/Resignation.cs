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
        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("resignationDate")]
        public DateTime ResignationDate { get; set; }
        [BsonElement("revealingDate")]
        public DateTime RevealingDate { get; set; }

        [BsonElement("reason")]
        public string? Reason { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set;}

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("approvedBy")]
        public string? ApprovedBy { get; set; }
    }
}
