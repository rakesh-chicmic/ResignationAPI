using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ResignationAPI.Models.DTOs
{
    public class ResignationStatusDTO
    {
        public int Status { get; set; }
        public DateTime RelievingDate { get; set; }
    }
}
