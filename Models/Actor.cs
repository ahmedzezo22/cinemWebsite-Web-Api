using System.ComponentModel.DataAnnotations;

namespace cinemaApp.Api.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public MovieActors MovieActors { get; set; }
        public int MovieActorsId { get; set; }
        public long MovieId { get; set; }
        public Movie Movie { get; set; }

    }
}