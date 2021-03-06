﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrionBlog.Data;
using OrionBlog.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _dbContext;
        private readonly MailSettings _mailSettings;

        public HomeController(ILogger<HomeController> logger,
            IEmailSender emailSender, 
            IOptions<MailSettings> mailSettings,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _emailSender = emailSender;
            _dbContext = dbContext;
            _mailSettings = mailSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            
            var categories = await _dbContext.BlogCategory.ToListAsync();
            return View(categories);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact contact)
        {
            await _emailSender.SendEmailAsync(_mailSettings.Mail, $" From {contact.FirstName} {contact.LastName} {contact.Email}", contact.Message);

            return RedirectToAction("index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
