using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Models
{
    public class CategoryPost
    {
        //Keys
        public int Id { get; set; }
        [Display(Name = "Category Name")]
        public int BlogCategoryId { get; set; }

        //Descriptive properties
        [Required]
        public string Title { get; set; }
        [Required]
        public string Abstract { get; set; }
        [Required]
        [Display(Name = "Blog Body")]
        public string PostBody { get; set; }
        [Display(Name = "Production Ready?")]
        public bool IsProductionReady { get; set; }

        //Programmatically derived properties
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime? Updated { get; set; }
        public string Slug { get; set; }

        //I need to add properties for storing images
        [Display(Name = "Choose Image")]
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }




        //Navigational properties:
        public virtual BlogCategory BlogCategory { get; set; }

        //Typical parent-child relationship (aka 1 to many relationship)
        public virtual ICollection<CommentPost> CommentPosts { get; set; } =
            new HashSet<CommentPost>();

        // Many to many relationship
        public virtual ICollection<Tag> Tags { get; set; } =
            new HashSet<Tag>();
    }
}
        
