using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.View_Models
{
    public class GroceryListListVm
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }

        [NotMapped, Display(Name = "Number Of Items")]
        public int NumberofItems { get; set; }

        [Display(Name = "OwnerEmail")]
        public string OwnerEmail { get; set; }

    }
}
