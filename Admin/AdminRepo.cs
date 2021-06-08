using System.Collections.Generic;
using System.Threading.Tasks;
using cinemaApp.Api.Data;
using cinemaApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using cinemaApp.Api.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
namespace cinemaApp.Api.Admin
{
    public class AdminRepo : IAdminRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        [System.Obsolete]
        private readonly IHostingEnvironment _host;
        [System.Obsolete]
        public AdminRepo(DataContext context, UserManager<AppUser> userManager, IHostingEnvironment host, RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _host = host;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<AppUser> AddUser(AddUserDto adduser)
        {
            if (adduser == null)
            {
                return null;
            }
            var user = new AppUser
            {
                UserName = adduser.UserName,
                Email = adduser.Email,
                EmailConfirmed = adduser.EmailConfirmed,
                Country = adduser.Country,
                PhoneNumber = adduser.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, adduser.Password);
            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<AppUser> GetUser(string id)
        {
            if (id == null)
            {
                return null;
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<AppUser> EditUser(EditUserModel model)
        {
            if (model.Id == null) { return null; }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user == null) { return null; }
            _context.Users.Attach(user);
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.EmailConfirmed;
            user.Country = model.Country;
            if (model.Password != user.PasswordHash)
            {
                var pass = await _userManager.RemovePasswordAsync(user);
                if (pass.Succeeded)
                    await _userManager.AddPasswordAsync(user, model.Password);
            }
            _context.Entry(user).Property(x => x.Email).IsModified = true;
            _context.Entry(user).Property(x => x.PhoneNumber).IsModified = true;
            _context.Entry(user).Property(x => x.Country).IsModified = true;
            _context.Entry(user).Property(x => x.UserName).IsModified = true;
            _context.Entry(user).Property(x => x.EmailConfirmed).IsModified = true;
            await _context.SaveChangesAsync();
            return user;

        }
        public async Task<bool> DeleteUsers(List<string> ids)
        {
            if (ids.Count < 1) return false;
            var i = 0;
            foreach (var id in ids)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) { return false; }

                _context.Users.Remove(user);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<IEnumerable<UserRoleModel>> GetUserRoleModels()
        {
            var query = await (from userRole in _context.UserRoles
                               join user in _context.Users
                               on userRole.UserId equals user.Id
                               join roles in _context.Roles
                               on
                               userRole.RoleId equals roles.Id
                               select new
                               {
                                   user.Id,
                                   user.UserName,
                                   userRole.RoleId,
                                   roles.Name
                               }
            ).ToListAsync();
            List<UserRoleModel> userRoleModels = new List<UserRoleModel>();
            if (query.Count > 0)
            {

                for (int i = 0; i < query.Count; i++)
                {
                    var model = new UserRoleModel
                    {
                        RoleId = query[i].RoleId,
                        UserName = query[i].UserName,
                        UserId = query[i].Id,
                        RoleName = query[i].Name
                    };
                    userRoleModels.Add(model);
                }
            }
            return userRoleModels;
        }

        public async Task<IEnumerable<AppRole>> GetAllRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> EditUserRoleModel(EditUserRoleModel model)
        {
            if (model.RoleId == null && model.UserId == null)
            {
                return false;
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user == null) { return false; }
            var CurrentRoleId = await _context.UserRoles.Where(x => x.UserId == model.UserId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var CurrentRoleName = await _context.Roles.Where(x => x.Id == CurrentRoleId).Select(x => x.Name).FirstOrDefaultAsync();
            var NewRoleName = await _context.Roles.Where(x => x.Id == model.RoleId).Select(x => x.Name).FirstOrDefaultAsync();

            if (await _userManager.IsInRoleAsync(user, CurrentRoleName))
            {
                var x = await _userManager.RemoveFromRoleAsync(user, CurrentRoleName);
                if (x.Succeeded)
                {
                    var s = await _userManager.AddToRoleAsync(user, NewRoleName);
                    if (s.Succeeded)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.Include(x => x.SubCategory).ToListAsync();

        }

        public async Task<Category> AddCategory(Category cat)
        {
            var category = new Category
            {
                CategoryName = cat.CategoryName
            };
            _context.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> EditCategory(Category cat)
        {
            if (cat == null && cat.Id < 1)
            {
                return null;
            }
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == cat.Id);
            if (category == null) { return null; }
            _context.Categories.Attach(category);
            category.CategoryName = cat.CategoryName;
            _context.Entry(category).Property(x => x.CategoryName).IsModified = true;
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(List<int> ids)
        {
            if (ids.Count < 1) return false;
            var i = 0;
            foreach (var id in ids)
            {
                var cat = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (cat == null) { return false; }

                _context.Categories.Remove(cat);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            return await _context.SubCategories.Include(x => x.Category).ToListAsync();
        }

        public async Task<SubCategory> AddSubCategory(SubCategory cat)
        {
            var category = new SubCategory
            {
                SubCategoryName = cat.SubCategoryName,
                CategoryId = cat.CategoryId,

            };
            _context.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<SubCategory> EditSubCategory(SubCategory cat)
        {

            if (cat == null && cat.Id < 1)
            {
                return null;
            }
            var category = await _context.SubCategories.FirstOrDefaultAsync(c => c.Id == cat.Id);
            if (category == null) { return null; }
            _context.SubCategories.Attach(category);
            category.SubCategoryName = cat.SubCategoryName;
            category.CategoryId = cat.CategoryId;
            // cat.Category.CategoryName=cat.Category.CategoryName;
            _context.Entry(category).Property(x => x.SubCategoryName).IsModified = true;
            _context.Entry(category).Property(x => x.CategoryId).IsModified = true;
            // _context.Entry(category).Property(x => x.Category).IsModified = true;
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<bool> DeleteSubCategory(List<int> ids)
        {
            if (ids.Count < 1) return false;
            var i = 0;
            foreach (var id in ids)
            {
                var cat = await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == id);
                if (cat == null) { return false; }

                _context.SubCategories.Remove(cat);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<IEnumerable<MovieActors>> GetAllActors()
        {
            return await _context.MovieActors.ToListAsync();

        }

        [System.Obsolete]
        public async Task<MovieActors> AddActor(string name, IFormFile img)
        {
            // var filePath=Path.Combine(_host.WebRootPath,"/User_Files/images/actors",img.FileName);
            // using(FileStream fileStream=new FileStream(filePath,FileMode.Create)){
            //     await img.CopyToAsync(fileStream);
            // }
            string UniqueFileName = null;
            if (img != null)
            {
                string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\actorsPicture");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + img.FileName;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                img.CopyTo(new FileStream(FilePath, FileMode.Create));
            }
            var actor = new MovieActors
            {
                ActorName = name,
                ActorPicture = UniqueFileName
            };
            _context.Add(actor);
            await _context.SaveChangesAsync();
            return actor;
        }
        public async Task<MovieActors> GetActor(int id)
        {
            var actor = await _context.MovieActors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return null;
            }
            return actor;
        }

        public async Task<bool> DeleteAllActors(List<int> ids)
        {
            if (ids.Count < 1) { return false; }
            var i = 0;
            foreach (var id in ids)
            {
                var actor = await _context.MovieActors.FirstOrDefaultAsync(x => x.Id == id);

                var path = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\actorsPicture\", actor.ActorPicture);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                if (actor == null) { return false; }
                _context.MovieActors.Remove(actor);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;

        }

        public async Task<MovieActors> EditActor(int id, string actorName, IFormFile actorPic)
        {
            var actor = await _context.MovieActors.FirstOrDefaultAsync(x => x.Id == id);
            _context.MovieActors.Attach(actor);
            actor.ActorName = actorName;
            string UniqueFileName = "";
            if (actorPic.FileName.ToLower() != actor.ActorPicture && actorPic != null)
            {
                var path = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\actorsPicture\", actor.ActorPicture);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\actorsPicture");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + actorPic.FileName;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                actorPic.CopyTo(new FileStream(FilePath, FileMode.Create));
                actor.ActorPicture = UniqueFileName;

                _context.Entry(actor).Property(x => x.ActorPicture).IsModified = true;
            }
            _context.Entry(actor).Property(x => x.ActorName).IsModified = true;
            await _context.SaveChangesAsync();

            return actor;
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _context.Movies.Include(x => x.SubCategory).ToListAsync();
        }

        // public async Task<Movie> AddMovie(AddMovieDto model)
        // {
        //     if (model == null) { return null; }
        //     string UniqueFileName = null;
        //     if (model.MoviePost != null)
        //     {
        //         string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\moviesPicture");
        //         UniqueFileName = Guid.NewGuid().ToString() + "_" + model.MoviePost.FileName;
        //         string FilePath = Path.Combine(UploadFolder, UniqueFileName);
        //         model.MoviePost.CopyTo(new FileStream(FilePath, FileMode.Create));
        //     }
        //     var movie = new Movie
        //     {
        //         MovieName = model.MovieName,
        //         MoviePost = UniqueFileName,
        //         MovieTrailer = model.MovieTrailer,
        //         MovieStory = model.MovieStory,
        //         SubCategoryId = model.SubCategoryId
        //     };
        //     _context.Add(movie);
        //     await _context.SaveChangesAsync();
        //     return movie;
        // }

        public async Task<Movie> GetMovie(int id)
        {
            if (id == 0)
            {
                return null;
            }
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
            {
                return null;
            }
            return movie;
        }

        public async Task<bool> DeleteMovies(List<int> ids)
        {
            if (ids.Count <= 0)
            {
                return false;
            }
            int i = 0;
            foreach (var id in ids)
            {
                var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
                if (movie == null) return false;
                var path = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\moviesPicture\", movie.MoviePost);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                _context.Movies.Remove(movie);
                i++;
            }
            if (i > 0) { await _context.SaveChangesAsync(); }
            return true;

        }

        public async Task<Movie> EditMovie(EditMovieDto editMovie)
        {
            if (editMovie == null) { return null; }
            var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == editMovie.Id);
            if (movie != null)
            {
                _context.Movies.Attach(movie);
                string UniqueFileName = "";
                if (editMovie.MoviePost.FileName.ToLower() != movie.MoviePost && editMovie.MoviePost != null)
                {
                    var path = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\moviesPicture\", movie.MoviePost);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\moviesPicture");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + editMovie.MoviePost.FileName;
                    string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                    editMovie.MoviePost.CopyTo(new FileStream(FilePath, FileMode.Create));
                    movie.MoviePost = UniqueFileName;
                    _context.Entry(movie).Property(x => x.MoviePost).IsModified = true;
                }
                movie.MovieName = editMovie.MovieName;
                movie.MovieStory = editMovie.MovieStory;
                movie.MovieTrailer = editMovie.MovieTrailer;
                movie.SubCategoryId = editMovie.SubCategoryId;
                _context.Entry(movie).Property(x => x.MovieName).IsModified = true;
                _context.Entry(movie).Property(x => x.MovieTrailer).IsModified = true;
                _context.Entry(movie).Property(x => x.MovieStory).IsModified = true;
                _context.Entry(movie).Property(x => x.SubCategoryId).IsModified = true;
                await _context.SaveChangesAsync();
            }
            return movie;
        }
        public async Task<bool> AddMovieAsync(IFormFile img, IFormFile video, string story, string movieName, string trailer,
                   string catId, List<int> ids, string[] links)
        {
            foreach (var mov in await _context.Movies.ToListAsync())
            {
                if (mov.MovieName.Trim().ToLower() == movieName.Trim().ToLower())
                {
                    return false;
                }
            }
            string UniqueFileName = null;
            if (img != null)
            {
                string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\moviesPicture");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + img.FileName;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                img.CopyTo(new FileStream(FilePath, FileMode.Create));
            }

            var movie = new Movie
            {
                MovieName = movieName,
                MovieStory = story,
                MovieTrailer = trailer,
                MoviePost = UniqueFileName,
                SubCategoryId = int.Parse(catId)
            };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            foreach (var id in ids)
            {
                var movieActors = new Actor
                {
                    MovieId = movie.Id,
                    MovieActorsId = id

                };
                _context.Actors.Add(movieActors);
                await _context.SaveChangesAsync();
            }
            string UniqueFileName2 = null;
            if (video != null)
            {
                string UploadFolder = Path.Combine(@"F:\cinemaApp\cinemaApp-SPA\src\assets\videos");
                UniqueFileName2 = Guid.NewGuid().ToString() + "_" + video.FileName;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName2);
                video.CopyTo(new FileStream(FilePath, FileMode.Create));
            }
            var movieLink = new MovieLink
            {
                MovLink = UniqueFileName2,
                MovieId = movie.Id
            };
            _context.MovieLinks.Add(movieLink);
            await _context.SaveChangesAsync();

            if (links.Count() >= 0)
            {
                for (int i = 0; i < links.Count(); i++)
                {
                    var movieLink2 = new MovieLink
                    {
                        MovLink = links[i],
                        MovieId = movie.Id
                    };
                    _context.MovieLinks.Add(movieLink2);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            return true;
        }
        public async Task<IEnumerable<Movie>> SearchMovie(string name)
        {
            if (name == null) { return null; }
            var mov = await _context.Movies.Include(x => x.SubCategory).
            Where(x => x.MovieName.ToLower().Contains(name.ToLower())
            || x.SubCategory.SubCategoryName.ToLower().Contains(name.ToLower()))
            .ToListAsync();
            if (mov == null) { return null; }
            return mov;
        }

        public async Task<IEnumerable<MovieLink>> GetAllMovieLinks()
        {
            return await _context.MovieLinks.Include(x => x.Movie).ThenInclude(x => x.SubCategory).ToListAsync();
        }

        public async Task<bool> DeleteLinks(List<int> ids)
        {
            if (ids.Count() < 0) { return false; }
            int i = 0;
            foreach (var id in ids)
            {
                var movLink = await _context.MovieLinks.FirstOrDefaultAsync(x => x.Id == id);
                if (movLink == null) { return false; }
                _context.MovieLinks.Remove(movLink);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<IEnumerable<MovieLink>> SearchLink(string name)
        {
            if (name == null) { return null; }
            var movLink = await _context.MovieLinks.Include(x => x.Movie).ThenInclude(x => x.SubCategory).
            Where(x => x.MovLink.ToLower().Contains(name.ToLower()) || x.Movie.MovieName.ToLower().Contains(name.ToLower())
            || x.Movie.SubCategory.SubCategoryName.ToLower().Contains(name.ToLower())).ToListAsync();
            return movLink;
        }
        public async Task<MovieLink> AddMovieLink(AddMovieLinkDto model)
        {
            if (model == null) { return null; }
            var movLink = new MovieLink
            {
                MovLink = model.MovLink,
                Quality = model.Quality,
                Resolution = model.Resolution,
                MovieId = model.MovieId,

            };
            _context.Add(movLink);
            await _context.SaveChangesAsync();
            return movLink;
        }
        public async Task<MovieLink> GetMovieLink(int id)
        {
            if (id == 0) { return null; }
            var movLink = await _context.MovieLinks.Include(x => x.Movie).FirstOrDefaultAsync(x => x.Id == id);
            if (movLink == null)
            {
                return null;
            }
            return movLink;
        }

        public async Task<MovieLink> EditMovieLink(EditMovieLinkDto model)
        {
            if (model == null) { return null; }
            var link = await _context.MovieLinks.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (link == null) { return null; }
            else
            {
                link.Id = model.Id;
                link.MovieId = model.MovieId;
                link.MovLink = model.MovLink;
                link.Quality = model.Quality;
                link.Resolution = model.Resolution;
                _context.Update(link);
                await _context.SaveChangesAsync();
            }
            return link;
        }

        public async Task<IEnumerable<Actor>> getAllMovieActors()
        {
            return await _context.Actors.Include(x => x.MovieActors).Include(x => x.Movie).ThenInclude(x => x.SubCategory).ToListAsync();
        }

        public async Task<IEnumerable<Actor>> SearchMovieActors(string search)
        {
            if (search == null)
            { return null; }
            var movActors = await _context.Actors.Include(x => x.MovieActors).Include(x => x.Movie).ThenInclude(x => x.SubCategory).Where(
              x => x.Movie.MovieName.ToLower().Contains(search.ToLower()) || x.Movie.SubCategory.SubCategoryName.ToLower().Contains(search.ToLower()) ||
              x.MovieActors.ActorName.ToLower().Contains(search.ToLower())
            ).ToListAsync();
            if (movActors == null) { return null; }
            return movActors;
        }
        public async Task<bool> DeleteMovieActors(List<int> ids)
        {
            if (ids.Count < 0) { return false; }
            int i = 0;
            foreach (var id in ids)
            {
                var movLink = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
                if (movLink == null) { return false; }
                _context.Remove(movLink);
                i++;
            }
            if (i > 0)
            {
                await _context.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> AddMovieActor(AddMovieActorDtos model)
        {
            if (model == null) return false;
            Actor actor = new()
            {
                MovieId = model.MovieId,
                MovieActorsId = model.MovieActorsId
            };
            _context.Add(actor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Actor> getMovieActorById(int id)
        {
            if (id < 1) { return null; }
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null) { return null; }
            return actor;
        }

        public async Task<bool> EditMovieActor(EditMovieActorDto editMovieActorDto)
        {
            if (editMovieActorDto == null) { return false; }
            var ActorEdit = await _context.Actors.FirstOrDefaultAsync(x => x.Id == editMovieActorDto.Id);
            if (ActorEdit == null) { return false; }
            ActorEdit.Id = editMovieActorDto.Id;
            ActorEdit.MovieId = editMovieActorDto.MovieId;
            ActorEdit.MovieActorsId = editMovieActorDto.MovieActorsId;
            var result = _context.Actors.Update(ActorEdit);
            if (result != null)
            {
                await _context.SaveChangesAsync();
                return true;
            }
            return false;

        }
        public  int CountCategories(){ return _context.Categories.Count();}
          public  int CountMovies(){return _context.Movies.Count();}
         public  int CountUsers(){return _context.Users.Count();}
         public  int CountActors(){return _context.MovieActors.Count();}
         public  int CountSubCategories(){ return _context.SubCategories.Count(); }
          public  int CountLinks(){ return _context.MovieLinks.Count(); }
    }
}
