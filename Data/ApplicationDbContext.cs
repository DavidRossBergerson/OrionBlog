using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrionBlog.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrionBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<BlogUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogCategory> BlogCategory { get; set; }
        public DbSet<CategoryPost> CategoryPost { get; set; }
        public DbSet<CommentPost> CommentPost { get; set; }
        public DbSet<Tag> Tag { get; set; }
    }
}
