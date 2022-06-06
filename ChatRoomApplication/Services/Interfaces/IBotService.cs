using System.Threading.Tasks;

namespace ChatRoomApplication.Services.Interfaces
{
    public interface IBotService
    {
        Task<string> GetStockData(string stockCode);

    }
}
