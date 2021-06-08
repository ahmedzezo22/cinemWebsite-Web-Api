using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Models
{
    public class MovieActors
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string ActorName { get; set; }
        public string ActorPicture { get; set; }

        public IList<Actor> Actor { get; set; }
        
        
    }
}