using cinemaApp.Api.Data;
using cinemaApp.Api.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace cinemaApp.Api.HomeRepository
{
    public class HomeRepo :IHomeRepo
    {
        private readonly DataContext _context;
        public HomeRepo(DataContext context)
        {
            _context=context;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesSearch(string name)
        {
            if(name==null || string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)){
                return await _context.Movies.Include(x => x.SubCategory).ThenInclude(x=>x.Category).ToListAsync();
            }
            
            return await _context.Movies.Include(x => x.SubCategory).ThenInclude(x=>x.Category).
            Where(x=>x.MovieName.ToLower().Contains(name.ToLower()) || x.SubCategory.SubCategoryName.ToLower().Contains(name.ToLower())
            ).ToListAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategories(){
               return await _context.SubCategories.ToListAsync();
        }
         public async Task<IEnumerable<Movie>> GetAllMovies(){
             return await _context.Movies.Include(x => x.SubCategory).ThenInclude(x=>x.Category).ToListAsync();
         }
          public async Task<MovieModel> GetMovieById(long Id){
             var movie= await _context.Movies.Include(x=>x.SubCategory).ThenInclude(x=>x.Category).FirstOrDefaultAsync(x=>x.Id==Id);
             if(movie==null)return null;
             var Links=await _context.MovieLinks.Where(x=>x.MovieId==movie.Id).ToListAsync();
             var MovieActors=await _context.Actors.Include(x=>x.MovieActors).Where(x=>x.MovieId==movie.Id).ToListAsync();
             var MovieModel=new MovieModel{
                 Movie=movie,
                 MovieLinks=Links,
                 MovieActors=MovieActors
             };
             return MovieModel;
             
         }
    
    }
}