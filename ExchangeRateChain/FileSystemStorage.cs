
namespace ExchangeRateChain
{
    public class FileSystemStorage<T> : IStorage<T>
    {
        private readonly string filePath = "exchange_rates.json";
        public bool IsReadOnly => false;
        public TimeSpan Expiration { get; } = TimeSpan.FromHours(4);          // The duration after which data should be considered expired.

        public bool ShouldCheckExpiration => true;
        public DateTime LastFetchTime { get; set; }

        public async Task<T> GetDataAsync()
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            var json = await File.ReadAllTextAsync(filePath);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        // Asynchronously saves data to a JSON file and updates the last fetch time to reflect the update time.
        public async Task SetDataAsync(T newData)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(newData);
            await File.WriteAllTextAsync(filePath, json);
            LastFetchTime = DateTime.Now;
        }
    }


}
