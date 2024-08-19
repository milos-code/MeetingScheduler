using System.Net;

namespace StudentRecords.Bussines.Exceptions
{
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ApiException(string message)
            : base(message)
        {
        }
    }
}
