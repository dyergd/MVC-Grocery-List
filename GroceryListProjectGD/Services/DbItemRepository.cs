using GroceryListProjectGD.Data;
using GroceryListProjectGD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the DBItemRepository class.
  This class fills out the method stubs from IItemRepository.
  The AddItemToList adds created item to the grocery list it is associated with and also adds it to the database.
  The ReadItem method reads a single item from the database.
  The DeleteItem method deletes the item form the grocery list it is associated with and also deletes it from the database.
*/
namespace GroceryListProjectGD.Services
{
    public class DbItemRepository : IItemRepository
    {
        private readonly IGroceryRepository groceryRepo;
        private readonly ApplicationDbContext db;

        public DbItemRepository(ApplicationDbContext dbContext, IGroceryRepository groceryRepository)
        {
            db = dbContext;
            groceryRepo = groceryRepository;
        }

        public Item AddItemToList(int groceryId, Item item)
        {
            var groceryList = groceryRepo.Read(groceryId);
            groceryList._item.Add(item);
            db.Items.Add(item);
            db.SaveChanges();
            return item;
        }

        public Item ReadItem(int id)
        {
            return db.Items.FirstOrDefault(i => i.Id == id);
        }

        public async Task DeleteItem(int id)
        {
            var itemToDelete = ReadItem(id);
            var groceryList = groceryRepo.Read(itemToDelete.GroceryListId);
            groceryList._item.Remove(itemToDelete);
            db.Items.Remove(itemToDelete);
            await db.SaveChangesAsync();
        }
    }
}
