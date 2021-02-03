using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Models
{
    public class CommentPost
    {
        //Keys
        public int Id { get; set; }
        public int CategoryPostId { get; set; }
        public string BlogUserId { get; set; }

        //Descriptive properties
        [Required]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long.", MinimumLength = 5)]
        public string CommentBody { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; }
        [Display(Name = "Last Updated")]
        public DateTime? Updated { get; set; }
        [Display(Name = "Last Moderated")]
        public DateTime? Moderated { get; set; }
        public string ModerationReason { get; set; }
        public string ModeratedBody { get; set; }

        //Nav properties
        public virtual CategoryPost CategoryPost { get; set; }
        public virtual BlogUser BlogUser { get; set; }
    }
}

