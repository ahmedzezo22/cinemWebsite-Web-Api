using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinemaApp.Api.Models
{
    public class MovieLink
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Quality { get; set; }

        public int Resolution { get; set; }

        [Required]
        public string MovLink { get; set; }

        public long MovieId { get; set; }

        public Movie Movie { get; set; }

    }
}