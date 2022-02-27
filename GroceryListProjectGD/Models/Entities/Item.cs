using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.Entities
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public int GroceryListId { get; set; }

        public GroceryList GroceryList { get; set; }

    }
}
