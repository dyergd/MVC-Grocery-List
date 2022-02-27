using GroceryListProjectGD.Models.Entities;
using GroceryListProjectGD.Models.View_Models;
using GroceryListProjectGD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*This is the Grocery Controller. This class provides webpages that allows users to create, edit, add permissions, and delete grocery lists.   
  It also allows the users to go grocery shopping.
  The CreateAjax method allows the user to create grocery lists.
  The Edit method allows the users to edit the name of the grocery list and add items.
  The DeleteAjax method allows the owner to delete the grocery list from their account.
  The Permission method reads all the users that have access to the grocery list.
  The Grant and Revoke permission methods grant and revoke permissions to the grocery list.
  The Go Shopping method reads all the items in the grocery list and allows the user to check them off their list.
  The GroceryRow method is used to automatically update the list of grocery lists.  
*/
namespace GroceryListProjectGD.Controllers
{
    public class GroceryController : Controller
    {
        private readonly IGroceryRepository groceryRepo;
        private readonly IUserRepository userRepo;

        public GroceryController(IGroceryRepository groceryRepository, IUserRepository userRepository)
        {
            groceryRepo = groceryRepository;
            userRepo = userRepository;
        }
            

         [HttpPost, ValidateAntiForgeryToken]
         public async Task<IActionResult> CreateAjax([FromForm]GroceryList groceryList)
         {
            if (ModelState.IsValid)
            {
                groceryRepo.CreateGroceryList(User.Identity.Name, groceryList);
                return Json(new { id = groceryList.Id, message = "created" });
            }

            return Json(ModelState);
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var user = userRepo.Details(User.Identity.Name);
            
            var groceryList = groceryRepo.Read(id);

            if (groceryList == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ViewData["Grocery"] = groceryList;
            var groceryListRole = groceryList._groceryListRoles;
            ViewData["GroceryRole"] = groceryListRole;
            ViewData["User"] = user;
            return View(groceryList);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit([FromForm] GroceryList groceryList)
        {

            if (ModelState.IsValid)
            {
                groceryRepo.EditGroceryName(groceryList.Id, groceryList);
                return RedirectToAction("Edit", "Grocery", new {id = groceryList.Id});
            }

            var user = userRepo.Details(User.Identity.Name);
            ViewData["Grocery"] = groceryList;
            var groceryListRole = groceryList._groceryListRoles;
            ViewData["GroceryRole"] = groceryListRole;
            ViewData["User"] = user;
            return View(groceryList);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var user = userRepo.Details(User.Identity.Name);
            var groceryList = groceryRepo.Read(id);
            var guest = new GroceryListRole();
            foreach(var i in groceryList._groceryListRoles)
            {
                if(i.Guest ==true)
                {
                    guest = i;
                }
            }
            
            if(guest.UserId == user.Id)
            {
                return Redirect("~/Identity/Account/AccessDenied");
            }
            else if (ModelState.IsValid)
            {
                await groceryRepo.Delete(id);
                return Json(new { id, message = "deleted" });
            }

            return RedirectToAction("Index", "Home");
        }



        [Authorize]
        public IActionResult Permissions(int id)
        {
            var user = userRepo.Details(User.Identity.Name);
            var groceryList = groceryRepo.Read(id);

            if (groceryList == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var thisGroceryListRole = groceryList._groceryListRoles.Where(r => r.GroceryListId == groceryList.Id);
            var singleGroceryRole = thisGroceryListRole.Where(o => o.Owner == true);

            foreach (var i in singleGroceryRole)
            {
                if (i.UserId != user.Id)
                {
                    return Redirect("~/Identity/Account/AccessDenied");
                }
            }

            var model = thisGroceryListRole.Select(u => 
                    new PermissionsUserVm
                    {
                        Email = u.Email, 
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    }
            );

            ViewData["Grocery"] = groceryList;
            ViewData["User"] = user;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult GrantPermissions([FromForm] string Email, int GroceryId)
        {
            groceryRepo.GrantPermissions(Email, GroceryId);
            return RedirectToAction("Permissions" , "Grocery" , new { id = GroceryId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult RevokePermissions([FromForm] string Email, int GroceryId)
        {
            var groceryList = groceryRepo.Read(GroceryId);
            groceryRepo.RevokePermissions(Email, groceryList);
            return RedirectToAction("Permissions", "Grocery", new { id = GroceryId });
        }



        [Authorize]
        public IActionResult GoShopping(int id)
        {
            var groceryList = groceryRepo.Read(id);
            ViewData["Grocery"] = groceryList;
            var user = userRepo.Details(User.Identity.Name);
            ViewData["User"] = user;
            var itemList = groceryList._item;
            return View(itemList);

        }

        public async Task<IActionResult> GroceryRow(int id)
        {
            var groceryList = await groceryRepo.ReadAsync(id);
            var user = userRepo.Details(groceryList.UserId);

            var model = new GroceryListListVm
            { 
                Id = groceryList.Id,
                Name = groceryList.Name,
                OwnerEmail = user.Email,
                NumberofItems = groceryList.NumberofItems
            };

            return PartialView("/Views/Home/_GroceryRow.cshtml", model);
        }

       




    }
}
