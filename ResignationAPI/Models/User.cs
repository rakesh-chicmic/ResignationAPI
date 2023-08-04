using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace ResignationAPI.Models
{
    public class User
    {
        [JsonProperty("name")]
        public string? Name { get; set; } 
        [JsonProperty("email")]
        public string? Email { get; set; } 
        [JsonProperty("employeeId")]
        public string? EmployeeId { get; set; } 
        [JsonProperty("hubstaffEmail")]
        public string? HubstaffEmail { get; set; } 
    }
}
