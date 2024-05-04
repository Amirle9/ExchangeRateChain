
namespace ExchangeRateChain
{
    // Implements IStorage for retrieving data from a web service.
    public class WebServiceStorage<T> : IReadableStorage<T>
    {
        private readonly string url;
        private readonly IDataConverter<T> converter;

        public WebServiceStorage(string url, IDataConverter<T> converter)
        {
            this.url = url;
            this.converter = converter;
        }

        public bool IsReadOnly => true;
        public TimeSpan Expiration => TimeSpan.MaxValue; // Irrelevant for this storage
        public DateTime LastFetchTime { get; set; }

        // Asynchronously fetches data from a specified web service URL and deserializes it into type T.
        public async Task<T> GetDataAsync()
        {
            using var client = new HttpClient();
            string response = await client.GetStringAsync(url);
            return converter.Deserialize(response);
        }

        public Task SetDataAsync(T data)
        {
            throw new InvalidOperationException("This storage is read-only.");
        }
    }


}
