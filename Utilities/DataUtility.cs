using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OrionBlog.Data;
using OrionBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrionBlog.Utilities
{

    public static class DataUtility
    {
        public static string GetConnectionString(IConfiguration configuration)
        {
            //The default connection string will come from appSettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //It will be automatically overwritten if we are running on Heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        private static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to the parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and manage the contents of connection strings used by the NpgsplConnection class.
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        public static async Task ManageDataAsync(IHost host)
        {
            //This technique is used to obtain references to services that get registered in the
            //ConfigureServices method of the Startup class
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;

            //This dbContextSvc knows how to talk to the DB (aka _context)
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            //Service 1: An instance RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service 2: An instance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

            //TsTEP 1: This is the programmatic equivalent to Update-Database
            await dbContextSvc.Database.MigrateAsync();
            //Step 1: Add a few Roles into the system (Administrator & Moderator)
            await SeedRolesAsync(roleManagerSvc);

            //Step 2: add a few Users
            await SeedUsersAsync(userManagerSvc);

            //Step 3: Assign a User to the Administrator role and a User to the Moderator Role
            await AssignRolesAsync(userManagerSvc);

        }


        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleSvc)
        {
            //Call upon the roleSvc to add a new Role
            await roleSvc.CreateAsync(new IdentityRole("Administrator"));
            await roleSvc.CreateAsync(new IdentityRole("Moderator"));

        }

        private static async Task SeedUsersAsync(UserManager<BlogUser> userManagerSvc)
        {
            //Step 1: Create yourself as a user
            var adminUser = new BlogUser()
            {
                Email = "davidrossbergerson@gmail.com",
                UserName = "davidrossbergerson@gmail.com",
                FirstName = "david",
                LastName = "bergerson",
                PhoneNumber = "352-538-7327",
                EmailConfirmed = true
            };

            await userManagerSvc.CreateAsync(adminUser, "Drb040189!");

            //Step 2: Create someone else as a user
            var modUser = new BlogUser()
            {
                Email = "davidbergerson@rocketmail.com",
                UserName = "davidbergerson@rocketmail.com",
                FirstName = "ross",
                LastName = "davidson",
                PhoneNumber = "352-538-7327",
                EmailConfirmed = true
            };

            await userManagerSvc.CreateAsync(modUser, "Abc&123!");

        }

        private static async Task AssignRolesAsync(UserManager<BlogUser> userManagerSvc)
        {
            //Write code here to assign each user to a specific role
           
            var adminUser = await userManagerSvc.FindByEmailAsync("davidrossbergerson@gmail.com");

            await userManagerSvc.AddToRoleAsync(adminUser, "Administrator");


            var modUser = await userManagerSvc.FindByEmailAsync("davidbergerson@rocketmail.com");

            await userManagerSvc.AddToRoleAsync(modUser, "Moderator");
        }

    }
}







