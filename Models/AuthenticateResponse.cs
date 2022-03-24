
namespace InvictaPartnersAPI.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(User user, string token)
        {
            Id = user.id;
            FirstName = user.firstName;
            LastName = user.lastName;
            Username = user.userName;
            Token = token;
        }
    }
}