using System.Threading.Tasks;
using cinemaApp.Api.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cinemaApp.Api.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using cinemaApp.Api.Models;
namespace cinemaApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _repo;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public AdminController(IAdminRepository repo, RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _repo = repo;
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();
            if (users == null)
            {

                return NotFound("لايوجد مستتخدمين حنى الان");
            }

            return Ok(users);
        }
        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(AddUserDto model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var user = await _repo.AddUser(model);
                if (user != null)
                {
                    if (await _roleManager.RoleExistsAsync("User"))
                    {
                        if (!await _userManager.IsInRoleAsync(user, "User") && !await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            await _userManager.AddToRoleAsync(user, "User");
                        }
                    }
                    return Ok(user);
                }
            }
            return BadRequest("حدث خطأ ما حاول مره اخرى");
        }
        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _repo.GetUser(id);
            if (user == null) { return NotFound(); }
            return Ok(user);
        }
        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            if (model == null) { return BadRequest(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _repo.EditUser(model);
            if (user != null)
            {
                return Ok();
            }
            return BadRequest("حدث خطأ ما");

        }
        [HttpPost("DeleteUsers")]
        public async Task<IActionResult> DeleteUsers(List<string> ids)
        {
            if (ids.Count < 1) { return BadRequest(); }
            var result = await _repo.DeleteUsers(ids);
            if (result) { return Ok(); }
            return BadRequest();
        }
        [HttpGet("GetUsersRoles")]
        public async Task<IActionResult> GetUsersRoles()
        {
            var userRoles = await _repo.GetUserRoleModels();
            if (userRoles == null) { return NotFound(); }

            return Ok(userRoles);
        }
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllsRoles()
        {
            var userRoles = await _repo.GetAllRoles();
            if (userRoles == null) { return NotFound(); }
            return Ok(userRoles);
        }
        [HttpPut("editUserRole")]
        public async Task<IActionResult> editUserRole(EditUserRoleModel editUserRole)
        {
            if (ModelState.IsValid)
            {
                var x = await _repo.EditUserRoleModel(editUserRole);
                if (x)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var cat = await _repo.GetAllCategories();
            if (cat == null) { return NotFound(); }
            return Ok(cat);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(Category cat)
        {
            if (cat == null) { return BadRequest(); }
            var category = await _repo.AddCategory(cat);
            if (category != null)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("EditCategory")]
        public async Task<IActionResult> EditCategory(Category cat)
        {
            if (cat == null) { return BadRequest(); }
            var category = await _repo.EditCategory(cat);
            if (category != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(List<int> ids)
        {
            if (ids.Count < 1) { return BadRequest(); }
            var result = await _repo.DeleteCategory(ids);
            if (result) { return Ok(); }
            return BadRequest();
        }
        [HttpGet("GetAllSubCategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var cat = await _repo.GetAllSubCategories();
            if (cat == null) { return NotFound(); }
            return Ok(cat);
        }

        [HttpPost("AddSubCategory")]
        public async Task<IActionResult> AddSubCategory(SubCategory cat)
        {
            //   if(!ModelState.IsValid){return BadRequest(ModelState.ValidationState);}
            if (cat == null) { return BadRequest(); }
            var category = await _repo.AddSubCategory(cat);
            if (category != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPut("EditSubCategory")]
        public async Task<IActionResult> EditSubCategory(SubCategory cat)
        {
            if (cat == null) { return BadRequest(); }
            var category = await _repo.EditSubCategory(cat);
            if (category != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost("DeleteSubCategory")]
        public async Task<IActionResult> DeleteSubCategory(List<int> ids)
        {
            if (ids.Count < 1) { return BadRequest(); }
            var result = await _repo.DeleteSubCategory(ids);
            if (result) { return Ok(); }
            return BadRequest();
        }
        [HttpGet("GetAllActors")]
        public async Task<IActionResult> GetAllActors()
        {
            var actors = await _repo.GetAllActors();
            if (actors == null) { return NotFound(); }
            return Ok(actors);
        }
        [HttpPost("AddActor")]
        public async Task<IActionResult> AddActor()
        {
            var img = HttpContext.Request.Form.Files["actorPicture"];
            var name = HttpContext.Request.Form["actorName"];
            var actor = await _repo.AddActor(name, img);

            if (actor == null)
            {
                return NoContent();
            }
            return Ok(actor);
        }
        [HttpGet("GetActor/{id}")]
        public async Task<IActionResult> GetActor(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var actor = await _repo.GetActor(id);
            if (actor != null)
            {
                return Ok(actor);
            }
            return BadRequest();
        }
        [HttpPost("DeleteAllActors")]
        public async Task<IActionResult> DeleteAllActors(List<int> ids)
        {
            if (ids == null)
            {
                return NotFound();
            }
            var DeleteActor = await _repo.DeleteAllActors(ids);
            if (DeleteActor)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPut("EditActor")]
        public async Task<IActionResult> EditActor()
        {
            int id = int.Parse(HttpContext.Request.Form["id"].ToString());
            var actorName = HttpContext.Request.Form["actorName"];
            var actorPic = HttpContext.Request.Form.Files["actorPicture"];
            var actor = await _repo.EditActor(id, actorName, actorPic);

            if (actor == null)
            {
                return NoContent();
            }
            return Ok(actor);
        }
        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _repo.GetAllMovies();
            if (movies != null)
            {
                return Ok(movies);
            }
            return NotFound();
        }

        // [HttpPost("AddMovie")]
        // public async Task<IActionResult> AddMovie([FromForm]AddMovieDto addMovieDto){
        //     if(addMovieDto==null){
        //         return BadRequest();
        //     }
        //     var movie=await _repo.AddMovie(addMovieDto);
        //     if(movie!=null){
        //         return Ok(movie);
        //     }
        //     return BadRequest();
        // }
        [Route("AddMovie")]
        [HttpPost]
        public async Task<IActionResult> AddMovie()
        {
            var img = HttpContext.Request.Form.Files["moviePost"];
            var video = HttpContext.Request.Form.Files["video"];
            var story = HttpContext.Request.Form["movieStory"].ToString();
            var movieName = HttpContext.Request.Form["movieName"].ToString();
            var trailer = HttpContext.Request.Form["movieTrailer"].ToString();
            var catId = HttpContext.Request.Form["catId"].ToString();
            var actorsId = HttpContext.Request.Form["actorsId[]"].ToArray();
            var links = HttpContext.Request.Form["links[]"].ToArray();
            List<int> ids = new List<int>();
            for (int i = 0; i < actorsId.Length; i++)
            {
                var result = int.TryParse(actorsId[i], out int id);
                if (result)
                    ids.Add(id);
            }

            if (ids.Count < 1)
            {
                return NoContent();
            }

            if (img != null && video != null && !string.IsNullOrEmpty(story) && !string.IsNullOrEmpty(movieName) && !
                string.IsNullOrEmpty(trailer) && !string.IsNullOrEmpty(catId) && ids.Count > 0)
            {
                var result = await _repo.AddMovieAsync(img, video, story, movieName, trailer, catId, ids, links);
                if (result)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpGet("GetMovie/{id}")]
        public async Task<IActionResult> GetMovie(int id)
        {
            if (id == 0) { return BadRequest(); }
            var movie = await _repo.GetMovie(id);
            if (movie != null)
            {
                return Ok(movie);
            }
            return BadRequest();
        }
        [HttpPost("DeleteMovie")]
        public async Task<IActionResult> DeleteMovie(List<int> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }
            var DeleteMovie = await _repo.DeleteMovies(ids);
            if (DeleteMovie) { return Ok(); }
            return BadRequest();
        }
        [HttpPut("EditMovie")]
        public async Task<IActionResult> EditMovie([FromForm] EditMovieDto editMovie)
        {
            if (editMovie == null)
            {
                return BadRequest();
            }
            var movie = await _repo.EditMovie(editMovie);
            if (movie != null)
            {
                return Ok(movie);
            }
            return BadRequest();
        }

        [HttpGet("SearchMovie/{name}")]
        public async Task<IActionResult> SearchMovie(string name)
        {
            if (name == null) { return NotFound(); }
            var mov = await _repo.SearchMovie(name);
            if (mov != null)
            {
                return Ok(mov);
            }
            return NotFound();
        }
        [HttpGet("GetAllMovieLinks")]
        public async Task<IActionResult> GetAllMovieLinks()
        {

            var movLink = await _repo.GetAllMovieLinks();
            if (movLink == null)
            {
                return NoContent();
            }
            return Ok(movLink);
        }
        [HttpPost("DeleteLinks")]
        public async Task<IActionResult> DeleteLinks(List<int> ids)
        {
            if (ids.Count <= 0) { return BadRequest(); }
            var movLink = await _repo.DeleteLinks(ids);
            if (!movLink)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("SearchLinks/{name}")]
        public async Task<IActionResult> SearchLinks(string name)
        {
            if (name == null)
            {
                return NoContent();
            }
            var movLink = await _repo.SearchLink(name);
            if (movLink == null)
            {
                return NoContent();
            }
            return Ok(movLink);
        }
        [HttpPost("AddMovieLinks")]
        public async Task<IActionResult> AddMovieLinks(AddMovieLinkDto model)
        {
            if (model == null) { return BadRequest(); }
            var movLink = await _repo.AddMovieLink(model);
            if (movLink == null)
            {
                return BadRequest();
            }
            return Ok(movLink);
        }
        [HttpGet("GetMovieLink/{id}")]
        public async Task<IActionResult> GetMovieLink(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var movLink = await _repo.GetMovieLink(id);
            if (movLink == null)
            {
                return BadRequest();
            }
            return Ok(movLink);
        }
        [HttpPut("EditMovieLink")]
        public async Task<IActionResult> EditMovieLink(EditMovieLinkDto model)
        {
            if (model == null) { return BadRequest(); }
            var LinkUpdate = await _repo.EditMovieLink(model);
            if (LinkUpdate == null)
            {
                return BadRequest();
            }
            return Ok();
        }
        [HttpGet("getAllMovieActors")]
        public async Task<IActionResult> getAllMovieActors()
        {
            var movieActors = await _repo.getAllMovieActors();
            if (movieActors == null) { return NoContent(); }
            return Ok(movieActors);
        }
        [HttpGet("SearchMovieActors/{search}")]
        public async Task<IActionResult> SearchMovieActors(string search)
        {
            if (search == null) { return BadRequest(); }
            var movActors = await _repo.SearchMovieActors(search);
            if (movActors == null) return BadRequest();

            return Ok(movActors);
        }
        [HttpPost("DeleteMovieActors")]
        public async Task<IActionResult> DeleteMovieActors(List<int> ids)
        {
            if (ids.Count <= 0)
                return BadRequest();
            var movActors = await _repo.DeleteMovieActors(ids);
            if (!movActors) return BadRequest();
            return Ok();
        }
        [HttpPost("AddMovieActor")]
        public async Task<IActionResult> AddMovieActor(AddMovieActorDtos model)
        {
            if (model == null) { return BadRequest(); }
            var movActors = await _repo.AddMovieActor(model);
            if (!movActors) { return BadRequest(); }
            return Ok();

        }
        [HttpGet("getMovieActorById/{id}")]
        public async Task<IActionResult> getMovieActorById(int id)
        {
          if(id<1){return BadRequest();}
          var movActor=await _repo.getMovieActorById(id);
          if(movActor==null){return BadRequest();}
          return Ok(movActor);
          
        }
        [HttpPut("EditMovieActor")]
        public async Task<IActionResult> EditMovieActor(EditMovieActorDto editMovieActorDto)
        {
              if(editMovieActorDto.Id<1){return BadRequest();}
              var ActorEdit=await _repo.EditMovieActor(editMovieActorDto);
              if(!ActorEdit) return BadRequest();
              return Ok();
        
        }
        [HttpGet("CountCategories")]
        public IActionResult CountCategories(){
                var Categories=_repo.CountCategories();
                return Ok(Categories);
        }
         [HttpGet("CountSubCategories")]
        public IActionResult CountSubCategories(){
                var SubCategories=_repo.CountSubCategories();
                return Ok(SubCategories);
        }
         [HttpGet("CountMovies")]
        public IActionResult CountMovies(){
                var movies=_repo.CountMovies();
                return Ok(movies);
        }
         [HttpGet("CountUsers")]
        public IActionResult CountUsers(){
                var users=_repo.CountUsers();
                return Ok(users);
        }
         [HttpGet("CountActors")]
        public IActionResult CountActors(){
                var actors=_repo.CountActors();
                return Ok(actors);
        }
         [HttpGet("CountLinks")]
        public IActionResult CountLinks(){
                var links=_repo.CountLinks();
                return Ok(links);
        }
    }

}
