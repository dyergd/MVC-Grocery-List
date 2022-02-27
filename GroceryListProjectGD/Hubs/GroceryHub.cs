using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace GroceryListProjectGD.Hubs
{
    public class GroceryHub : Hub
    {

        public async Task SendMessageToAllAsync()
        {
            await Clients.All.SendAsync("Notification");
        }

    }
}
