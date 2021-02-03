using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Models
{
    public class Tag
    {
        //Keys
        public int Id { get; set; }

        //Descriptive properties
        [Required]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }
        
        //Navigational properties
        public virtual ICollection<CategoryPost> CategoryPosts { get; set; } =
            new HashSet<CategoryPost>();
    }
}
