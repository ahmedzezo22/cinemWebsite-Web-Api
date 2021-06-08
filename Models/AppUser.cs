using Microsoft.AspNetCore.Identity;

namespace cinemaApp.Api.Models
{
    public class AppUser:IdentityUser
    {
        public string Country { get; set; }
        public string Gender { get; set; }
        
    }
}