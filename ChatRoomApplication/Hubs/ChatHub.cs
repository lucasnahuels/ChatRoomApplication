using System;
using System.Threading.Tasks;
using ChatRoomApplication.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApplication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IBotService _botService;
        public ChatHub(IBotService botService)
        {
            _botService = botService;
        }
        public async Task SendMessage(string user, string message)
        {
            user = user.Substring(0, user.IndexOf('@'));
            if (message.ToLower().StartsWith("/stock="))
            {
                var stockCode = message.Substring(message.IndexOf('=') + 1);
                user = "BOT";
                message = await _botService.GetStockData(stockCode);
            }
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}