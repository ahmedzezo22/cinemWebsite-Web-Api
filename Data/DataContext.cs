using cinemaApp.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace cinemaApp.Api.Data
{
    public class DataContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public DataContext(DbContextOptions<DataContext> option):base(option)
        {  
        }
       public DbSet<Category> Categories { get; set; }
       public DbSet<SubCategory> SubCategories { get; set; }
       public DbSet<Movie> Movies { get; set; }
       public DbSet<Actor> Actors { get; set; }
       public DbSet<MovieActors> MovieActors { get; set; }
       public DbSet<MovieLink> MovieLinks { get; set; }
       
       
       
       
       
       
       
       
    
        
        
    }
    
    
}