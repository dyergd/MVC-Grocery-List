using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.Entities
{
    public class GroceryList
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [NotMapped]
        public int NumberofItems { get; set; }

        public string UserId { get; set; }

        public ICollection<Item> _item { get; set; } = new List<Item>();

        public ICollection<GroceryListRole> _groceryListRoles { get; set; } 

        
        

        public GroceryList()
        {
            _item = new List<Item>();
            _groceryListRoles = new List<GroceryListRole>();
           
        }

    }
}
