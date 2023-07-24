﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ResignationAPI.Models.DTOs
{
    public class ResignationStatusDTO
    {
        public string? Status { get; set; }
        public DateTime RevailingDate { get; set; }
        public string? ApprovedBY { get; set;}
    }
}