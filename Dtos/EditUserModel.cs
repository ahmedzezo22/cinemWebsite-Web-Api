using System.ComponentModel.DataAnnotations;
namespace cinemaApp.Api.Dtos
{
    public class EditUserModel
    {
         public string Id { get; set; }
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
        
        public bool EmailConfirmed { get; set; }
        [DataType(DataType.PhoneNumber)]

        public string PhoneNumber { get; set; }
         [StringLength(256)]
        public string Country { get; set; }
    }
}