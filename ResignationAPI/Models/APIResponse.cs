using System.Net;

namespace ResignationAPI.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Messages { get; set; }
        public object Data { get; set; }
    }
}
