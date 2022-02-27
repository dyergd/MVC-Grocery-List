using GroceryListProjectGD.Data;
using GroceryListProjectGD.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the DbGroceryRepository. 
 * This class fills out the method stubs from the IGroceryRepository.
 * The Create Grocery List method creates a grocerylist and sets the user that created it to owner. It then adds the Grocery list to the database.
 * The Edit Grocery name method updates the grocery name as specified by the user and updates it in the database.
 * The Grant Permissions method allows the owner to give access to their grocery list to other users.
 * The Revoke Permissions method allows the owner to revoke other users access to their grocery list.
 * The Read Method reads a single grocery list from the database.
 * The ReadAllGroceryList method reads all grocery lists in the database that the current user owns, and where the user is guest on a grocery list they don't own.
 * The Delete method deletes a grocery list from the database.
 * The ReadAsync method reads a single grocery list from the database.
*/
namespace GroceryListProjectGD.Services
{
    public class DbGroceryRepository : IGroceryRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IUserRepository userRepo;

        public DbGroceryRepository(ApplicationDbContext dbContext, IUserRepository userRepository)
        {
            db = dbContext;
            userRepo = userRepository;
        }

        public  GroceryList CreateGroceryList(string userName, GroceryList groceryList)
        {
            var user = userRepo.Details(userName);

            var groceryListRole = new GroceryListRole
            {
                ApplicationUser = user,
                GroceryList = groceryList,
                GroceryListId = groceryList.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                GroceryListRoleId = 0,
                Guest = false,
                Owner = true
            };

            groceryList.UserId = user.Id;
            user.UserGroceryListRoles.Add(groceryListRole);           
            groceryList._groceryListRoles.Add(groceryListRole);
            db.GroceryLists.Add(groceryList);
            db.SaveChanges();
            return groceryList;
        }

        public void EditGroceryName(int oldId, GroceryList groceryList)
        {
            GroceryList groceryListToUpdate = Read(oldId);
            groceryListToUpdate.Name = groceryList.Name;
            db.SaveChanges();
        }

        public void GrantPermissions(string userName, int GroceryId)
        {
            var user = userRepo.Details(userName);
            var groceryList = Read(GroceryId);

            var groceryListRole = new GroceryListRole
            {
                ApplicationUser = user,
                GroceryList = groceryList,
                GroceryListId = groceryList.Id,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                GroceryListRoleId = 0,
                Guest = true,
                Owner = false
            };

            groceryList._groceryListRoles.Add(groceryListRole);
            user.UserGroceryListRoles.Add(groceryListRole);
            db.SaveChanges();
        }

        public void RevokePermissions(string userName, GroceryList groceryList)
        {
            var user = userRepo.Details(userName);
            var groceryRole = user.UserGroceryListRoles.Where(b => b.GroceryListId == groceryList.Id);
            GroceryListRole groceryListRole = new GroceryListRole();

            foreach (var i in groceryRole)
            {
                groceryListRole.GroceryList = i.GroceryList;
                groceryListRole.ApplicationUser = i.ApplicationUser;
                groceryListRole.GroceryListId = i.GroceryListId;
                groceryListRole.GroceryListRoleId = i.GroceryListRoleId;
                groceryListRole.Email = i.Email;
                groceryListRole.FirstName = i.FirstName;
                groceryListRole.LastName = i.LastName;
                groceryListRole.Guest = i.Guest;
                groceryListRole.Owner = i.Owner;
                groceryListRole.UserId = i.UserId;                   
            }

            user.UserGroceryListRoles.Remove(groceryListRole);
            groceryList._groceryListRoles.Remove(groceryListRole);
            db.GroceryListRoles.Remove(groceryListRole);
            db.SaveChanges();
        }

        public GroceryList Read(int id)
        {
            return db.GroceryLists.Include(gl => gl._groceryListRoles).ThenInclude(gl => gl.GroceryList)
                .Include(i => i._item).FirstOrDefault(g => g.Id == id);
        }

        public ICollection<GroceryList> ReadAllGroceryLists(string userName)
        {
            var user = userRepo.Details(userName);

            return db.GroceryLists.Include(gl => gl._groceryListRoles.Where(u => u.UserId == user.Id)).ThenInclude(gl => gl.GroceryList)
                .Include(g => g._item).Where(i => i.UserId == user.Id).ToList();
        }
        public async Task Delete(int id)
        {
            var groceryListToDelete = Read(id);
            db.GroceryLists.Remove(groceryListToDelete);
            await db.SaveChangesAsync();
        }

        public async Task<GroceryList> ReadAsync(int id)
        {
            var groceryList = await db.GroceryLists.FirstOrDefaultAsync(q => q.Id == id);
            return groceryList;
        }
    }
}
