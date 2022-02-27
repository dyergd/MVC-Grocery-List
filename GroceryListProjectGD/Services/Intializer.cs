using GroceryListProjectGD.Data;
using GroceryListProjectGD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Services
{
    public class Intializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Intializer(
           ApplicationDbContext db,
           UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedUsersAsync()
        {
            _db.Database.EnsureCreated();

            if (!_db.Users.Any(u => u.UserName == "admin@test.com"))
            {
                var user = new ApplicationUser
                {
                    Email = "admin@test.com",
                    UserName = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Admin"
                };
                await _userManager.CreateAsync(user, "Pass1!");
            }

         


        }






    }
}
