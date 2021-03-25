using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrionBlog.Data;
using OrionBlog.Models;

namespace OrionBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoryPosts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryPost>>> GetCategoryPost()
        {
            return await _context.CategoryPost.ToListAsync();
        }

        // GET: api/CategoryPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryPost>> GetCategoryPost(int id)
        {
            var categoryPost = await _context.CategoryPost.FindAsync(id);

            if (categoryPost == null)
            {
                return NotFound();
            }

            return categoryPost;
        }


        [HttpGet("/GetTopXPosts/{num}")]
        public async Task<ActionResult<IEnumerable<CategoryPost>>> GetTopXPosts(int num)
        {
            return await _context.CategoryPost.OrderByDescending(p => p.Created).Where(p => p.IsProductionReady).Take(num).ToListAsync();
        }

  
    }
}
