using ChatRoomApplication.Exceptions;
using ChatRoomApplication.Models;
using ChatRoomApplication.Services;
using ChatRoomApplication.Services.Interfaces;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ChatRoomApplication.Tests
{
    public class BotServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        public BotServiceTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public void GetStockData_IfStockCodeExists_ReturnsMessage()
        {
            //arrange
            string stockCode = "aapl.us";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Symbol,Date,Time,Open,High,Low,Close,Volume\r\nAAPL.US,2022 - 06 - 06,15:46:46,147.03,148.31,147.03,147.56,6024286\r\n"),
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var botService = new BotService(_httpClientFactory.Object);
            
            //act
            var response = botService.GetStockData(stockCode);

            //assert
            Assert.Equal("AAPL.US quote is 147.56 per share", response.Result);
        }

        [Theory]
        [InlineData("Symbol,Date,Time,Open,High,Low,Close,Volume\r\nAAPL.US,2022 - 06 - 06,15:46:46,147.03,148.31,147.03,147.56,6024286\r\n")]
        public void MapResponseToStockModel_IfApiResponseReturnsData_ResponseIsMapped(string apiResponse)
        {
            //arrange
            var botService = new BotService(_httpClientFactory.Object);
            var expectedResponse = new Stock
            {
                Symbol = "AAPL.US",
                Date = new DateTime(2022, 6, 6),
                Time = new DateTime(2022, 6, 6, 15, 46, 46).TimeOfDay,
                Open = 147.03,
                High = 148.31,
                Low = 147.03,
                Close = 147.56,
                Volume = 6024286,
            };

            //act
            var response = botService.MapResponseToStockModel(apiResponse);

            //assert
            Assert.Equal(expectedResponse.Symbol, response.Symbol);
            Assert.Equal(expectedResponse.Date, response.Date);
            Assert.Equal(expectedResponse.Time, response.Time);
            Assert.Equal(expectedResponse.Open, response.Open);
            Assert.Equal(expectedResponse.High, response.High);
            Assert.Equal(expectedResponse.Low, response.Low);
            Assert.Equal(expectedResponse.Close, response.Close);
            Assert.Equal(expectedResponse.Volume, response.Volume);
        }

        [Theory]
        [InlineData("Symbol,Date,Time,Open,High,Low,Close,Volume\r\nAAAPL.US,N/D,N/D,N/D,N/D,N/D,N/D,N/D\r\n")]
        public void MapResponseToStockModel_IfApiResponseReturnsNoData_ThrowEmptyException(string apiResponse)
        {
            //arrange
            var botService = new BotService(_httpClientFactory.Object);

            //assert
            Assert.Throws<EmptyException>(() => botService.MapResponseToStockModel(apiResponse));
        }
    }
}
