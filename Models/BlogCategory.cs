using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Models
{
    public class BlogCategory
    {
        //Keys
        public int Id { get; set; }

        //Descriptive properties
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name= "Created Date")]
        public DateTime Created { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Last Updated")]
        public DateTime? Updated { get; set; }

        //Navigational properties
        public virtual ICollection<CategoryPost> CategoryPosts { get; set; } =
            new HashSet<CategoryPost>();
    }
}
        

        
