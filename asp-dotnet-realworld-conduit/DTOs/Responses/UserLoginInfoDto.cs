using System;
namespace asp_dotnet_realworld_conduit.DTOs.Responses
{
    public class UserLoginInfoDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
