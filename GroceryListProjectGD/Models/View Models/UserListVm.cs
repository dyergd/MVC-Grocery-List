using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryListProjectGD.Models.View_Models
{
    public class UserListVm
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public int NumberofRoles { get; set; }
        public string RoleNames { get; set; }

    }
}
