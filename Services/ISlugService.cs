using OrionBlog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Services
{
    public interface ISlugService
    {
        //Method not property
        string URLFriendly(string title);

        bool IsUnique(ApplicationDbContext dbContext, string slug);
    }
}
