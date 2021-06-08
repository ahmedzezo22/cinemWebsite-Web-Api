using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        
        
        
        
        
        
    }
}