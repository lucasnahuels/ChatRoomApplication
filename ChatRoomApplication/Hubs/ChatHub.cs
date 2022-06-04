using System;
using System.Threading.Tasks;
using ChatRoomApplication.Helpers;
using Microsoft.AspNetCore.SignalR;

namespace ChatRoomApplication.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            if (message.ToLower().StartsWith("/stock="))
                try
                {
                    var stockCode = message.Substring(message.IndexOf('=') + 1);
                    user = "BOT";
                    message = await GetUrlHelper.GetStockData(stockCode);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}