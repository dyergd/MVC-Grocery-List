using GroceryListProjectGD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the IUserRepository. 
 * This creates the method stubs for any class that uses the interface.
 * These stubs tell the program what type it needs to return and and types each method can accept.
 * This class contains the method stubs for the different methods of reading users.
*/
namespace GroceryListProjectGD.Services
{
    public interface IUserRepository
    {
        Task<IQueryable<ApplicationUser>> ReadAllAsync();

        Task<ApplicationUser> ReadAsync(string userName);

        ApplicationUser Details(string name);
    }
}
