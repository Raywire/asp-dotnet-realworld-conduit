using System;
namespace Conduit.DTOs.Responses
{
    public class ProfileResponseDto
    {
        public string UserName { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
        public bool Following { get; set; }
    }
}
