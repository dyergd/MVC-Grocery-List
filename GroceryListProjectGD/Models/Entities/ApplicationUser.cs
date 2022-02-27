using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }


        public ICollection<GroceryListRole> UserGroceryListRoles { get; set; }

        public ApplicationUser()
        {
            UserGroceryListRoles = new List<GroceryListRole>();

        }

    }
}
