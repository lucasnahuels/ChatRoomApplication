using ChatRoomApplication.Exceptions;
using ChatRoomApplication.Models;
using ChatRoomApplication.Services.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatRoomApplication.Services
{

    public class BotService : IBotService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BotService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetStockData(string stockCode)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                httpClient.BaseAddress = new Uri("https://stooq.com/q/l/");
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync($"?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
                using (HttpContent content = response.Content)
                {
                    try
                    {
                        string stringContent = content.ReadAsStringAsync().Result;
                        if (response.StatusCode != HttpStatusCode.OK)
                            throw new Exception(stringContent);
                        var stock = MapResponseToStockModel(stringContent);
                        return $"{stock.Symbol} quote is {stock.Close} per share";
                    }
                    catch (EmptyException)
                    {
                        return $"'{stockCode}' quote was not found";
                    }
                    catch(Exception)
                    {
                        return "an error has ocurred during the operation";
                    }
                    }
            }
        }

        private Stock MapResponseToStockModel(string stringContent)
        {
            string data = stringContent.Substring(stringContent.IndexOf(Environment.NewLine, StringComparison.Ordinal) + 2);
            string[] dataColumns = data.Split(',');
            if (dataColumns.ToList().Skip(1).Count(c => c == "N/D" || c == "N/D\r\n") == dataColumns.ToList().Skip(1).Count()) throw new EmptyException();
            return new Stock
            {
                Symbol = dataColumns[0] != "N/D" ? dataColumns[0] : default,
                Date = dataColumns[1] != "N/D" ? Convert.ToDateTime(dataColumns[1]) : default,
                Time = dataColumns[2] != "N/D" ? Convert.ToDateTime(dataColumns[2]).TimeOfDay : default,
                Open = dataColumns[3] != "N/D" ? double.Parse(dataColumns[3]) : default,
                High = dataColumns[4] != "N/D" ? double.Parse(dataColumns[4]) : default,
                Low = dataColumns[5] != "N/D" ? double.Parse(dataColumns[5]) : default,
                Close = dataColumns[6] != "N/D" ? double.Parse(dataColumns[6]) : default,
                Volume = dataColumns[7] != "N/D" ? double.Parse(dataColumns[7]) : default,
            };
        }
    }
}
