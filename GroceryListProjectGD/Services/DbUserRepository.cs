using GroceryListProjectGD.Data;
using GroceryListProjectGD.Models.Entities;
using GroceryListProjectGD.Models.View_Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the DBUserRepository class.
  This class fills out the method stubs from IUserRepository.
  The ReadAllAsync method reads all the users from the database.
  The ReadAsync method reads a single user from the database.
  The Details method also reads a single user from the database.    
*/
namespace GroceryListProjectGD.Services
{
    public class DbUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext dbContext;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> roleManager;

        public DbUserRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> _roleManager)
        {
            dbContext = db;
            _userManager = userManager;
            roleManager = _roleManager;
            
        }

        public async Task<IQueryable<ApplicationUser>> ReadAllAsync()
        {
            var users = dbContext.Users;     
            return users;
        }

        public async Task<ApplicationUser> ReadAsync(string userName)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);          
            return user;
        }

        public ApplicationUser Details(string name)
        {
            return dbContext.Users.Include(g => g.UserGroceryListRoles).ThenInclude(gl => gl.GroceryList).
                FirstOrDefault(p => p.NormalizedUserName == name.ToUpper());
        }
    }
}
