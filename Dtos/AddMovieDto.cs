using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace cinemaApp.Api.Dtos
{
    public class AddMovieDto
    {
        [Required]
        public string MovieName { get; set; }
        [Required]
        public string MovieTrailer { get; set; }
        [Required]
        public IFormFile MoviePost { get; set; }
        [Required]
        public string MovieStory { get; set; }
        
        public int SubCategoryId { get; set; }

        // public SubCategory SubCategory { get; set; }
    }
}