using OrionBlog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Services
{
    public class DifferentSlugService : ISlugService
    {
        public bool IsUnique(ApplicationDbContext dbContext, string slug)
        {
            return !dbContext.CategoryPost.Any(cp => cp.Slug == slug);
        }

        public string URLFriendly(string title)
        {
            return title.ToLower();
        }
    }
}
