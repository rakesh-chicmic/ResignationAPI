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
        public int Status { get; set; }

        [BsonElement("resignDate")]
        public DateTime ResignDate { get; set; }
        [BsonElement("revelationDate")]
        public DateTime RevelationDate { get; set; }

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
