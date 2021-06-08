using System.Collections.Generic;
using System.Threading.Tasks;
using cinemaApp.Api.Models;
namespace cinemaApp.Api.HomeRepository
{
    public interface IHomeRepo
    {
        
         Task<IEnumerable<SubCategory>> GetAllSubCategories();
         Task<IEnumerable<Movie>> GetAllMoviesSearch(string name);
         Task<IEnumerable<Movie>>GetAllMovies();
         Task<MovieModel> GetMovieById(long Id);
       
    }
}