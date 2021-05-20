using System;
namespace Conduit.DTOs.Responses
{
    public class ProfileResponseDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Photo { get; set; }
        public bool Following { get; set; }
    }
}
