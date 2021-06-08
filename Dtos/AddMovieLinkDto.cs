using System.ComponentModel.DataAnnotations;
namespace cinemaApp.Api.Dtos
{
    public class AddMovieLinkDto
    {
     public string Quality { get; set; }

        public int Resolution { get; set; }

        [Required]
        public string MovLink { get; set; }

        public long MovieId { get; set; }

    }
}