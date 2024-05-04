
namespace ExchangeRateChain
{
    public class FileSystemStorage<T> : IWritableStorage<T>
    {
        private readonly string filePath;
        private readonly IDataConverter<T> converter;

        public FileSystemStorage(string filePath, IDataConverter<T> converter)
        {
            this.filePath = filePath;
            this.converter = converter;
        }

        public bool IsReadOnly => false;
        public TimeSpan Expiration { get; } = TimeSpan.FromHours(4);    // The duration after which data should be considered expired.

        public DateTime LastFetchTime { get; set; }

        // Asynchronously retrieves data from the specified file path. Returns default(T) if the file does not exist.

        public async Task<T> GetDataAsync()
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            string data = await File.ReadAllTextAsync(filePath);
            return converter.Deserialize(data);
        }

        public async Task SetDataAsync(T newData)
        {
            string data = converter.Serialize(newData);
            await File.WriteAllTextAsync(filePath, data);
            LastFetchTime = DateTime.UtcNow;
        }
    }


}
