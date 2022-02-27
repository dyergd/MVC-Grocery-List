using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.Entities
{
    public class GroceryListRole
    {
        [Key]
        public int GroceryListRoleId { get; set; }

        public bool Owner { get; set; }

        public bool Guest { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string UserId { get; set; }

        public GroceryList GroceryList { get; set; }

        public int GroceryListId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }


    }
}
