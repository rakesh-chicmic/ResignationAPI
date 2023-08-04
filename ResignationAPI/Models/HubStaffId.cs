using MongoDB.Bson.Serialization.Attributes;

namespace ResignationAPI.Models
{
    public class HubStaffId
    {
        [BsonElement("email")]
        public string? Email { get; set; }
    }
}
