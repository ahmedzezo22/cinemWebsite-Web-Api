using System.Collections.Generic;
using System.Threading.Tasks;
using cinemaApp.Api.Models;
using cinemaApp.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace cinemaApp.Api.Admin
{
    public interface IAdminRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsers();
        Task<AppUser> AddUser(AddUserDto adduser);
        Task<AppUser> GetUser(string id);
        Task<AppUser> EditUser(EditUserModel model);
        Task<bool> DeleteUsers(List<string> ids);
        Task<IEnumerable<UserRoleModel>> GetUserRoleModels();
        Task<IEnumerable<AppRole>> GetAllRoles();
        Task<bool> EditUserRoleModel(EditUserRoleModel model);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> AddCategory(Category cat);
        Task<Category> EditCategory(Category cat);
        Task<bool> DeleteCategory(List<int> ids);
        Task<IEnumerable<SubCategory>> GetAllSubCategories();
        Task<SubCategory> AddSubCategory(SubCategory cat);
        Task<SubCategory> EditSubCategory(SubCategory cat);
        Task<bool> DeleteSubCategory(List<int> ids);
        Task<IEnumerable<MovieActors>> GetAllActors();
        Task<MovieActors> AddActor(string name,IFormFile img);
        Task<MovieActors> GetActor(int id);
        Task<bool> DeleteAllActors(List<int> ids);
        Task<MovieActors> EditActor(int id,string actorName, IFormFile actorPic);
        Task<IEnumerable<Movie>> GetAllMovies();
       // Task<Movie> AddMovie(AddMovieDto model);
        Task<Movie> GetMovie(int id);
        Task<bool> DeleteMovies(List<int> ids);
        Task<Movie> EditMovie(EditMovieDto editMovie);
        Task<bool> AddMovieAsync(IFormFile img, IFormFile video, string story, string movieName, string trailer, string catId, List<int> ids, string[] links);
        Task<IEnumerable<Movie>> SearchMovie(string name);
        Task<IEnumerable<MovieLink>> GetAllMovieLinks();
        Task<bool>DeleteLinks(List<int> ids);
        Task<IEnumerable<MovieLink>> SearchLink(string name);
        Task<MovieLink>AddMovieLink(AddMovieLinkDto model);
        Task<MovieLink>GetMovieLink(int id);
        Task<MovieLink>EditMovieLink(EditMovieLinkDto model);
        Task<IEnumerable<Actor>> getAllMovieActors();
        Task<IEnumerable<Actor>>SearchMovieActors(string search);
        Task<bool>DeleteMovieActors(List<int> ids);
        Task<bool> AddMovieActor(AddMovieActorDtos model);
        Task<Actor> getMovieActorById(int id);
        Task<bool> EditMovieActor(EditMovieActorDto editMovieActorDto);
        int CountCategories();
        int CountMovies();
        int CountUsers();
        int CountActors();
        int CountSubCategories();
        int CountLinks();
    }
}