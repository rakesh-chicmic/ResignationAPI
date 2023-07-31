namespace ResignationAPI.Models.DTOs
{
    public class ResignationDTO
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public int Status { get; set; }
        public DateTime ResignationDate { get; set; }
        public DateTime RelievingDate { get; set; }
        public string? Reason { get; set; }
        public string Comments { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ApprovedBy { get; set; }
    }
}
