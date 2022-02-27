using GroceryListProjectGD.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GroceryListProjectGD.Models.View_Models;
/*This is the ApplicationDbContext. 
  This class creates the dbsets that we use to interact with the database.
*/
namespace GroceryListProjectGD.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

            public DbSet<Item> Items { get; set; }
            public DbSet<GroceryList> GroceryLists { get; set; }

            public DbSet<GroceryListRole> GroceryListRoles { get; set; }                  
    }   
         
        
}


