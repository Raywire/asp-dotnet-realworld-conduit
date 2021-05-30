using System.Collections.Generic;

namespace Conduit.DTOs.Responses
{
    public class Errors
    {
        public string Message { get; set; }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public Errors Errors { get; set; }
    }

    public class ValidationErrorResponse
    {
        public bool Success { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
    }
}
