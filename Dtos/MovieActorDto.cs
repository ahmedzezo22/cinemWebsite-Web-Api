using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace cinemaApp.Api.Dtos
{
    public class MovieActorDto
    {
         [Required, StringLength(200)]
         public string ActorName { get; set; }
        public IFormFile ActorPicture { get; set; }

    }
}