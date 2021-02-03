using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public static async Task ManageDataAsync(IHost host)
        {
            //This technique is used to obtain references to services that get registered in the
            //ConfigureServices method of the Startup class
            using var svcScope = host.Services.CreateScope();
            var svcProvider = svcScope.ServiceProvider;

            //This dbContextSvc knows how to talk to the DB (aka _context)

            //Service 1: An instance RoleManager
            var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //Service 2: An instance of UserManager
            var userManagerSvc = svcProvider.GetRequiredService<UserManager<BlogUser>>();

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







