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
        public async Task<IActionResult> Index(int? pageNumber, string searchString)
        {
            ViewData["HeaderImage"] = @"~/img/home-bg.jpg";
            ViewData["SearchString"] = searchString;

            //I want to look at the incoming pageNumber variable and either usit it
            //Or force it to be 1 and then use 1
            //pageNumber ??= 1;
            pageNumber = pageNumber == null || pageNumber <= 0 ? 1 : pageNumber;
            ViewData["PageNumber"] = pageNumber;

            //Define an arbitrary page size
            

            int pageSize = 3, ttlRecords = 0, ttlPages = 0;


            

            //Either use the search string or not
            IQueryable<CategoryPost> result = null;
            if(!string.IsNullOrEmpty (searchString))
            {

                //I have to search my records for the presence of the search string
                result = _context.CategoryPost.AsQueryable();
                searchString = searchString.ToLower();

                result = result.Where(cp => cp.Title.ToLower().Contains(searchString) ||
                                            cp.PostBody.ToLower().Contains(searchString) ||
                                            cp.Abstract.ToLower().Contains(searchString) ||
                                            cp.CommentPosts.Any(c => c.CommentBody.ToLower().Contains(searchString) ||
                                                                     c.BlogUser.FirstName.ToLower().Contains(searchString) ||
                                                                     c.BlogUser.LastName.ToLower().Contains(searchString) ||
                                                                     c.BlogUser.DisplayName.ToLower().Contains(searchString) ||
                                                                     c.BlogUser.Email.ToLower().Contains(searchString)));


                ttlRecords = (await result.ToListAsync()).Count;
            }
            else
            {
                result = _context.CategoryPost.AsQueryable();
                ttlRecords = (await result.ToListAsync()).Count;
            }

          
             if((ttlRecords % pageSize) > 0)
            {
                ttlPages = Convert.ToInt32(ttlRecords / pageSize) + 1;
            }
            else
            {
                ttlPages = Convert.ToInt32(ttlRecords / pageSize);
            }

            
            ViewData["TtlPages"] = ttlPages;

            pageNumber = pageNumber > ttlPages ? ttlPages : pageNumber;

            if (ttlPages > 0)
            {
                //Define a sentence telling the User what page they're on
                ViewData["PageXofY"] = $"You are viewing page {pageNumber} of {ttlPages}";
            }
            else
            {
                ViewData["PageXofY"] = $"Your search string yielded no results";
            }

            

            var skipCount = ((int)pageNumber - 1) * pageSize;
            
            var posts = (await result.OrderByDescending(cp => cp.Created).ToListAsync()).Skip(skipCount).Take(pageSize);
            return View(posts);
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
