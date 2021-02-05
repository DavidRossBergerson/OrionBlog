using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrionBlog.Data;
using OrionBlog.Models;
using OrionBlog.Services;

namespace OrionBlog.Controllers
{
    public class CategoryPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ISlugService _slugService;
        private readonly IImageService _imageService;

        public CategoryPostsController(
            ApplicationDbContext context, 
            ISlugService slugService,
            IImageService imageService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
        }

        // GET: CategoryPosts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CategoryPost.Include(c => c.BlogCategory);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> BlogPosts(int? id)
        {
            return View("Index", await _context.CategoryPost.Where(p => p.BlogCategoryId == id).ToListAsync());
        }

       
        
        
        public IActionResult CategoryIndex(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }
            //Write a LINQ statement that uses the Id to get all of the Blog Posts with the CategoryId FK = id
            var posts = _context.CategoryPost.Where(cp => cp.BlogCategoryId == id).Include(c => c.BlogCategory).ToList();

            //Once I have my Blog posts I want to display them in the Index view
            return View("index", posts);
        }

        // GET: CategoryPosts/Details/5 Before adding custom route

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryPost = await _context.CategoryPost
        //        .Include(c => c.BlogCategory)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (categoryPost == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(categoryPost);
        //}

        // GET: CategoryPosts/Details/5  After adding custom route


        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var categoryPost = await _context.CategoryPost
                .Include(c => c.BlogCategory)
                .Include(c => c.CommentPosts)
                .ThenInclude(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (categoryPost == null)
            {
                return NotFound();
            }

            return View(categoryPost);
        }

        // GET: CategoryPosts/Create

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            //I have created a BlogCategoryId KEY in the ViewData dictionary
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name");
            return View();
        }

        // POST: CategoryPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BlogCategoryId,Title,Abstract,PostBody,IsProductionReady")] CategoryPost categoryPost, 
            IFormFile formFile)
        {
            if (ModelState.IsValid)
            {
                categoryPost.Created = DateTime.Now;

                //I can only work with the incoming image if it is not null


                categoryPost.ContentType = _imageService.RecordContentType(formFile);
                categoryPost.ImageData = await _imageService.EncodeFileAsync(formFile);
                
               
                var slug = _slugService.URLFriendly(categoryPost.Title);
                if (_slugService.IsUnique(_context, slug))
                {
                    categoryPost.Slug = slug;
                }
                else
                {
                    ModelState.AddModelError("Title", "This Title cannot be used as it results in a duplicate Slug!");
                    ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", categoryPost.BlogCategoryId);
                    return View(categoryPost);
                }



                _context.Add(categoryPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", categoryPost.BlogCategoryId);
            return View(categoryPost);

        }

        // GET: CategoryPosts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryPost = await _context.CategoryPost.FindAsync(id);
            if (categoryPost == null)
            {
                return NotFound();
            }
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", categoryPost.BlogCategoryId);
            return View(categoryPost);
        }

        // POST: CategoryPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogCategoryId,Title,Abstract,PostBody,IsProductionReady,Created,Slug")] CategoryPost categoryPost)
        {
            if (id != categoryPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var slug = _slugService.URLFriendly(categoryPost.Title);
                    if (slug != categoryPost.Slug)
                    {
                        if (_slugService.IsUnique(_context, slug))
                        {
                            categoryPost.Slug = slug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This Title cannot be used as it results in a duplicate Slug!");
                            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", categoryPost.BlogCategoryId);
                            return View(categoryPost);
                        }
                    }

                    categoryPost.Updated = DateTime.Now;
                    _context.Update(categoryPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryPostExists(categoryPost.Id))
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
            ViewData["BlogCategoryId"] = new SelectList(_context.BlogCategory, "Id", "Name", categoryPost.BlogCategoryId);
            return View(categoryPost);
        }

        // GET: CategoryPosts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryPost = await _context.CategoryPost
                .Include(c => c.BlogCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoryPost == null)
            {
                return NotFound();
            }

            return View(categoryPost);
        }

        // POST: CategoryPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryPost = await _context.CategoryPost.FindAsync(id);
            _context.CategoryPost.Remove(categoryPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryPostExists(int id)
        {
            return _context.CategoryPost.Any(e => e.Id == id);
        }
    }
}
