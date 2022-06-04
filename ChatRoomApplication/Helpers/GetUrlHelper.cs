﻿using ChatRoomApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatRoomApplication.Helpers
{
    public static class GetUrlHelper
    {
        public static async Task<string> GetStockData(string stockCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://stooq.com/q/l/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
                using (HttpContent content = response.Content)
                {
                    string stringContent = content.ReadAsStringAsync().Result;
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new ArgumentException(stringContent);

                    var stock = MapResponseToStockModel(stringContent);
                    return $"{stock.Symbol} quote is {stock.Close} per share";
                }
            }
        }

        private static Stock MapResponseToStockModel(string stringContent)
        {
            string data = stringContent.Substring(stringContent.IndexOf(Environment.NewLine, StringComparison.Ordinal) + 2);
            string[] dataColumns = data.Split(',');
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
