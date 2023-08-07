using System.Net;

namespace ResignationAPI.Models
{
    public class APIResponse
    {
     
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public bool Status { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public APIResponse ErrorResponse(string message= "Error Occured while processing in server. Check Out the logs", HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new APIResponse
            {
                StatusCode = statusCode,
                Status = false,
                Message = message,
                Data = null
            };
        }
    }

  
}
