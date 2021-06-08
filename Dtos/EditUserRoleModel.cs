using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Dtos
{
    public class EditUserRoleModel
    {
         [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleId { get; set; }

    }
}