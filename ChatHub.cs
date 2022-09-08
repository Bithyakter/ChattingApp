using ChattingApp.Models;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChattingApp
{
    public class ChatHub : Hub
    {
        public async void ShowData()
        {
           
            await Clients.All.SendAsync("RefreshChat");
        }
    }
}
