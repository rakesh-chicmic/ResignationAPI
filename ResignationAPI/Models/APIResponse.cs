using System.Net;

namespace ResignationAPI.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Status { get; set; } = false;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
