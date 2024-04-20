
namespace ExchangeRateChain
{
    // Implements the IStorage interface to provide an in-memory storage mechanism for data of type T.
    public class MemoryStorage<T> : IStorage<T>
    {
        private T? data;
        public bool IsReadOnly => false;
        public TimeSpan Expiration { get; } = TimeSpan.FromHours(1);
        public bool ShouldCheckExpiration => true;
        public DateTime LastFetchTime { get; set; }

        // Retrieves the data from memory if it has not expired; otherwise, returns default(T).
        public Task<T> GetDataAsync()
        {
            return DateTime.Now - LastFetchTime < Expiration ? Task.FromResult(data) : Task.FromResult(default(T));
        }

        public Task SetDataAsync(T newData)
        {
            data = newData;
            LastFetchTime = DateTime.Now;
            return Task.CompletedTask;
        }
    }


}
