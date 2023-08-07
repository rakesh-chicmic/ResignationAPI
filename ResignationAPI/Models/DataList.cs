namespace ResignationAPI.Models
{
    public class DataList
    {
        public List<ResignationWithUser> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
