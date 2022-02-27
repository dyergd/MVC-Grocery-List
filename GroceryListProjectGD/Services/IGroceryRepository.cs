using GroceryListProjectGD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the IGroceryRepository 
 * This creates the method stubs for any class that uses the interface.
 * These stubs tell the program what type it needs to return and and types each method can accept.
 * This class contains the method stubs for crud operations for Grocery Lists, and for granting and revoking permissions.
*/
namespace GroceryListProjectGD.Services
{
    public interface IGroceryRepository
    {
        GroceryList CreateGroceryList(string user, GroceryList groceryList);

        Task<GroceryList> ReadAsync(int id);

        ICollection<GroceryList> ReadAllGroceryLists(string userName);

        void GrantPermissions(string Email, int GroceryId);

        void RevokePermissions(string userName, GroceryList groceryList);

        GroceryList Read(int id);

        void EditGroceryName(int oldId, GroceryList groceryList);

        Task Delete(int id);
    }
}
