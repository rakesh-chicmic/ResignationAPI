﻿namespace ResignationAPI.Models.DTOs
{
    public class ResignationRequestDTO
    {
        public DateTime ResignDate { get; set; }
        public string? Reason { get; set; } 
        public string? Comments { get; set; } 
    }
}
