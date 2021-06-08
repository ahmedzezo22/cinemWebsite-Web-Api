using System.Collections.Generic;

namespace cinemaApp.Api.Models
{
    public class MovieModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<MovieLink> MovieLinks { get; set; }
        public IEnumerable<Actor> MovieActors { get; set; }
        
        
        
        
    }
}