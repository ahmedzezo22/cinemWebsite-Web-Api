using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace cinemaApp.Api.Dtos
{
    public class EditMovieDto
    {
        public int Id { get; set; }
    
         [Required]
        public string MovieName { get; set; }
        [Required]
        public string MovieTrailer { get; set; }
        [Required]
        public IFormFile MoviePost { get; set; }
        [Required]
        public string MovieStory { get; set; }
        
        public int SubCategoryId { get; set; }
    }
}