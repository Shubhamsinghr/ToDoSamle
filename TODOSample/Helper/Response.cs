using DAL;
using System.Collections.Generic;

namespace TODOSample
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object ResponseData { get; set; }

        public Response(bool status, string message, object data)
        {
            IsSuccess = status;
            Message = message;
            ResponseData = data;
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public bool IsSuperAdmin { get; set; }
    }

}
