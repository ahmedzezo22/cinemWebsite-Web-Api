using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Dtos
{
    public class ResetPassword
    {
        [Required]
        //[DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string ID { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
         
        public string Token { get; set; }
        
    }
}