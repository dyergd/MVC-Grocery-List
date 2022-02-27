using GroceryListProjectGD.Data;
using GroceryListProjectGD.Models.Entities;
using GroceryListProjectGD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the Item Controller. This class controls how various data is returned to the view.
  The Create Item method gets data from the item the user created and adds it to the grocery list.
  The Delete Item method calls the nessecary methods to delete the item from the database as well as deleting it from the grocerylist.  
  The Item Row method is used to automatically update the item list on the edit page.
*/
namespace GroceryListProjectGD.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IUserRepository userRepo;
        private readonly IGroceryRepository groceryRepo;
        private readonly IItemRepository itemRepo;

        public ItemController(IUserRepository userRepository, IGroceryRepository groceryRepository, IItemRepository itemRepository, 
            ApplicationDbContext dbContext)
        {
            userRepo = userRepository;
            groceryRepo = groceryRepository;
            itemRepo = itemRepository;
            db = dbContext;
        }
      
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem([FromForm]Item item, int groceryId)
        {
            if(ModelState.IsValid)
            {
                item.GroceryListId = groceryId;
                itemRepo.AddItemToList(groceryId, item);
                await db.SaveChangesAsync();
                return Json(new { id = item.Id, message = "created" });
            }

            return Json(ModelState);
        }

        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = itemRepo.ReadItem(id);
            var groceryList = groceryRepo.Read(item.GroceryListId);
            groceryList._item.Remove(item);
            itemRepo.DeleteItem(id);
            db.SaveChanges();
            return Json(new { id = id, message = "deleted" });
        }
        public async Task<IActionResult> ItemRow(int id)
        {
            var item = itemRepo.ReadItem(id);
            return PartialView("/Views/Grocery/_ItemRow.cshtml", item);
        }
    }
}
