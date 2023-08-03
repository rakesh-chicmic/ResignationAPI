using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ResignationAPI.Models
{
    public class ResignationWithUser 
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string? UserId { get; set; }
        [BsonElement("userDetails")]
        public object? UserDetails { get; set; }

        [BsonElement("status")]
        public int Status { get; set; }

        [BsonElement("resignationDate")]
        public DateTime ResignationDate { get; set; }
        [BsonElement("relievingDate")]
        public DateTime RelievingDate { get; set; }

        [BsonElement("reason")]
        public string? Reason { get; set; }

        [BsonElement("comments")]
        public string Comments { get; set; } = null!;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("approvedBy")]
        public string? ApprovedBy { get; set; }
        
    }
}
