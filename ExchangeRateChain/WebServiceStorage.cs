
namespace ExchangeRateChain
{
    // Implements IStorage for retrieving data from a web service.
    public class WebServiceStorage<T> : IStorage<T>
    {
        public bool IsReadOnly => true;
        public TimeSpan Expiration => TimeSpan.MaxValue; // Irrelevant for this storage
        public bool ShouldCheckExpiration => false;
        public DateTime LastFetchTime { get; set; }

        public async Task<T> GetDataAsync()
        {
            using var client = new HttpClient();
            string url = "https://openexchangerates.org/api/latest.json?app_id=22e9810ac7694a4da9c7a506f663a4f8";
            var response = await client.GetStringAsync(url);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response);
        }

        public Task SetDataAsync(T data)
        {
            throw new InvalidOperationException("This storage is read-only.");
        }
    }


}
