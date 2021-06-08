using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cinemaApp.Api.HomeRepository;

namespace cinemaApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class HomeController :ControllerBase
    {
        private readonly IHomeRepo _repo;
        public HomeController(IHomeRepo repo)
        {
            _repo=repo;
        }
        [HttpGet("GetAllSubCategories")]
        public async Task<IActionResult>  GetAllSubCategories(){
           var subCategories=await _repo.GetAllSubCategories();
           if(subCategories==null){return NoContent();}
           return Ok(subCategories);
        }
          [HttpGet("GetAllMovies/{search?}")]
        public async Task<IActionResult> GetAllMovies(string search)
        {
            if(string.IsNullOrEmpty(search)){return Ok(await _repo.GetAllMovies());}
            var movies = await _repo.GetAllMoviesSearch(search);
            return Ok(movies);
        }
        [HttpGet("GetMovieById/{id}")]
         public async Task<IActionResult> GetMovieById(long id){
                if(id<=0){
                  return BadRequest();
                }
                var movie=await _repo.GetMovieById(id);
                if(movie==null){
                    return BadRequest();
                }
                return Ok(movie);
         }
       
       
    }
}