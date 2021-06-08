using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace cinemaApp.Api.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public IList<Movie> Movies { get; set; }
        
        


    }
}
