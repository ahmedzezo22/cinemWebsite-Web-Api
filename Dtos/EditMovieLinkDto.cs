using System.ComponentModel.DataAnnotations;
namespace cinemaApp.Api.Dtos
{
    public class EditMovieLinkDto
    {
          public long Id { get; set; }
        public string Quality { get; set; }

        public int Resolution { get; set; }

        [Required]
        public string MovLink { get; set; }

        public long MovieId { get; set; }

    }
}