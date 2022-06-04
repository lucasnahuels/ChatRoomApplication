using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomApplication.Services.Interfaces
{
    public interface IBotService
    {
        Task<string> GetStockData(string stockCode);

    }
}
