using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Dtos
{
    public class UserRegisterDto
    {
        [StringLength(256)]
        [Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
         [StringLength(256)]
        public string UserName { get; set; }
        
        




    }
}