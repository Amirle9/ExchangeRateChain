
using Moq;
using ExchangeRateChain;

namespace Tests
{
    public class ChainResourceTests
    {
        // Test case to verify that GetValueAsync retrieves value from the first storage if it's not expired.
        [Fact]
        public async Task GetValueAsync_ReturnsValueFromFirstStorage_IfNotExpired()
        {
            // Arrange: 
            // Creating a new ExchangeRateList instance with sample data.
            var exchangeRateList = new ExchangeRateList
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };

            var memoryStorageMock = new Mock<IStorage<ExchangeRateList>>();
            memoryStorageMock.Setup(m => m.GetDataAsync())
                             .ReturnsAsync(exchangeRateList);
            // Simulating that the data was just fetched, thus not expired.
            memoryStorageMock.Setup(m => m.LastFetchTime)
                             .Returns(DateTime.Now);
            // Setting the expiration time to 1 hour from now.
            memoryStorageMock.Setup(m => m.Expiration)
                             .Returns(TimeSpan.FromHours(1));
            // Adding our mock memory storage to a list, to be passed to the ChainResource constructor.
            var storages = new List<IStorage<ExchangeRateList>> { memoryStorageMock.Object };
            var chainResource = new ChainResource<ExchangeRateList>(storages);


            // Act:
            // Calling GetValueAsync which should retrieve the exchange rate list from the mock memory storage.
            var result = await chainResource.GetValueAsync();


            // Assert
            Assert.NotNull(result);             // Verify that the result is not null, meaning data was retrieved successfully.
            Assert.Equal("USD", result.Base);   // Verify that the base currency is as expected, confirming the correct data was retrieved.
            memoryStorageMock.Verify(m => m.GetDataAsync(), Times.Once);    // Ensure that the GetDataAsync method on the mock memory storage was called exactly once.
        }

        // Test that GetValueAsync retrieves data from a secondary storage when the first storage's data has expired.
        [Fact]
        public async Task GetValueAsync_ReturnsValueFromFileSystemStorage_WhenMemoryStorageIsExpired()
        {
            // Arrange
            var exchangeRateList = new ExchangeRateList
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };

            var memoryStorageMock = new Mock<IStorage<ExchangeRateList>>();
            memoryStorageMock.Setup(m => m.GetDataAsync()).ReturnsAsync(default(ExchangeRateList));
            memoryStorageMock.Setup(m => m.LastFetchTime).Returns(DateTime.Now.Subtract(TimeSpan.FromHours(2)));
            memoryStorageMock.Setup(m => m.ShouldCheckExpiration).Returns(true);
            memoryStorageMock.Setup(m => m.Expiration).Returns(TimeSpan.FromHours(1));

            var fileSystemStorageMock = new Mock<IStorage<ExchangeRateList>>();
            fileSystemStorageMock.Setup(m => m.GetDataAsync()).ReturnsAsync(exchangeRateList);
            fileSystemStorageMock.Setup(m => m.LastFetchTime).Returns(DateTime.Now);
            fileSystemStorageMock.Setup(m => m.ShouldCheckExpiration).Returns(true);
            fileSystemStorageMock.Setup(m => m.Expiration).Returns(TimeSpan.FromHours(4));

            var storages = new List<IStorage<ExchangeRateList>> { memoryStorageMock.Object, fileSystemStorageMock.Object };
            var chainResource = new ChainResource<ExchangeRateList>(storages);


            // Act
            var result = await chainResource.GetValueAsync();


            // Assert:
            // Confirm that the data was retrieved successfully and from the correct storage.
            Assert.NotNull(result);
            Assert.Equal("USD", result.Base);
            memoryStorageMock.Verify(m => m.GetDataAsync(), Times.Never);
            fileSystemStorageMock.Verify(m => m.GetDataAsync(), Times.Once);
        }



        // Test for retrieval from a web service storage when all local storages are expired or empty.
        [Fact]
        public async Task GetValueAsync_ReturnsValueFromWebService_WhenFileSystemStorageIsExpired()
        {
            // Arrange
            var exchangeRateList = new ExchangeRateList
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "GBP", 0.75m } }
            };

            var memoryStorageMock = new Mock<IStorage<ExchangeRateList>>();
            memoryStorageMock.Setup(m => m.GetDataAsync()).ReturnsAsync(default(ExchangeRateList));
            memoryStorageMock.Setup(m => m.Expiration).Returns(TimeSpan.FromHours(1));
            memoryStorageMock.Setup(m => m.IsReadOnly).Returns(false);
            memoryStorageMock.Setup(m => m.ShouldCheckExpiration).Returns(true); 
            memoryStorageMock.SetupProperty(m => m.LastFetchTime, DateTime.Now.Subtract(TimeSpan.FromHours(2)));

            var fileSystemStorageMock = new Mock<IStorage<ExchangeRateList>>();
            fileSystemStorageMock.Setup(m => m.GetDataAsync()).ReturnsAsync(default(ExchangeRateList));
            fileSystemStorageMock.Setup(m => m.Expiration).Returns(TimeSpan.FromHours(4));
            fileSystemStorageMock.Setup(m => m.IsReadOnly).Returns(false);
            fileSystemStorageMock.Setup(m => m.ShouldCheckExpiration).Returns(true); 
            fileSystemStorageMock.SetupProperty(m => m.LastFetchTime, DateTime.Now.Subtract(TimeSpan.FromHours(5)));

            var webServiceStorageMock = new Mock<IStorage<ExchangeRateList>>();
            webServiceStorageMock.Setup(m => m.GetDataAsync()).ReturnsAsync(exchangeRateList);
            webServiceStorageMock.Setup(m => m.IsReadOnly).Returns(true);
            webServiceStorageMock.Setup(m => m.ShouldCheckExpiration).Returns(false); // WebService should not check expiration
            webServiceStorageMock.SetupProperty(m => m.LastFetchTime, DateTime.Now);

            var storages = new List<IStorage<ExchangeRateList>>
                {
                    memoryStorageMock.Object,
                    fileSystemStorageMock.Object,
                    webServiceStorageMock.Object
                };
            var chainResource = new ChainResource<ExchangeRateList>(storages);

            // Act
            var result = await chainResource.GetValueAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD", result.Base);
            webServiceStorageMock.Verify(m => m.GetDataAsync(), Times.Once);
            memoryStorageMock.Verify(m => m.GetDataAsync(), Times.Never); // Verify it is never called since it is expired
            fileSystemStorageMock.Verify(m => m.GetDataAsync(), Times.Never); // Verify it is never called since it is expired
        }

    }

}

