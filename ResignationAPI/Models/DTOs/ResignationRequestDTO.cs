namespace ResignationAPI.Models.DTOs
{
    public class ResignationRequestDTO
    {
        public string? UserId { get; set; }
        public DateTime ResignationDate { get; set; }
        public string? Reason { get; set; } 
        public string Details { get; set; } = null!;
    }
}
