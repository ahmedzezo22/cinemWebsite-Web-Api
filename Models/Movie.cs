using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinemaApp.Api.Models
{
    public class Movie
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required]
        public string MovieName { get; set; }
        [Required]
        public string MovieTrailer { get; set; }
        [Required]
        public string MoviePost { get; set; }
        [Required]
        public string MovieStory { get; set; }
        
        public int SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; }
        public IList<Actor> Actors { get; set; }
        
        public IList<MovieLink> MovieLinks { get; set; }
    }
}