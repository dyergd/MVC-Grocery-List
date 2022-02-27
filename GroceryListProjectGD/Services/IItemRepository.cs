using GroceryListProjectGD.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the IItemRepository.
 * This creates the method stubs for any class that uses the interface.
 * These stubs tell the program what type it needs to return and and types each method can accept.
 * This class contains the method stubs for reading items, deleting items, and adding items to a grocery list.
*/
namespace GroceryListProjectGD.Services
{
    public interface IItemRepository
    {
        Item AddItemToList(int groceryId, Item item);

        Item ReadItem(int id);

        Task DeleteItem(int id);       
    }
}
