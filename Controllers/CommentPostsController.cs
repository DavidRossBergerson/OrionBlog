using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrionBlog.Data;
using OrionBlog.Models;

namespace OrionBlog.Controllers
{
    public class CommentPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CommentPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CommentPost.Include(c => c.BlogUser).Include(c => c.CategoryPost);
            return View(await applicationDbContext.ToListAsync());
        }

    

       

        // POST: CommentPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryPostId,CommentBody")] CommentPost commentPost)
        {
            if (ModelState.IsValid)
            {
                commentPost.Created = DateTime.Now;

                //How do i tell my program that the Id of the Author of this comment is the Id of the User
                commentPost.BlogUserId = _userManager.GetUserId(User);
                _context.Add(commentPost);
                await _context.SaveChangesAsync();

                var slug = (await _context.CategoryPost.FindAsync(commentPost.CategoryPostId)).Slug;
                return RedirectToAction("Details", "CategoryPosts", new { slug });
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "UserName", commentPost.BlogUserId);
            ViewData["CategoryPostId"] = new SelectList(_context.CategoryPost, "Id", "Title", commentPost.CategoryPostId);
            return View(commentPost);
        }

        // GET: CommentPosts/Edit/5
        
        [Authorize(Roles = "Moderator, Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commentPost = await _context.CommentPost.FindAsync(id);
            if (commentPost == null)
            {
                return NotFound();
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "UserName", commentPost.BlogUserId);
            ViewData["CategoryPostId"] = new SelectList(_context.CategoryPost, "Id", "Title", commentPost.CategoryPostId);
            return View(commentPost);
        }

        // POST: CommentPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryPostId,BlogUserId,CommentBody,Created,Moderated,ModerationReason,ModeratedBody")] CommentPost commentPost)
        {
            if (id != commentPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    commentPost.Updated = DateTime.Now;
                    _context.Update(commentPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentPostExists(commentPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "UserName", commentPost.BlogUserId);
            ViewData["CategoryPostId"] = new SelectList(_context.CategoryPost, "Id", "Title", commentPost.CategoryPostId);
            return View(commentPost);
        }

        private bool CommentPostExists(int id)
        {
            return _context.CommentPost.Any(e => e.Id == id);
        }
    }
}
