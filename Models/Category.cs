using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace cinemaApp.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string CategoryName { get; set; }        

        public IList<SubCategory> SubCategory { get; set; }
        
        
    }
}