
namespace ExchangeRateChain
{
    // Implements the IStorage interface to provide an in-memory storage mechanism for data of type T.
    public class MemoryStorage<T> : IWritableStorage<T>
    {
        private T? data;     // Private variable to hold the actual data in memory.

        public bool IsReadOnly => false;
        public TimeSpan Expiration { get; } = TimeSpan.FromHours(1);
        public DateTime LastFetchTime { get; set; }

        // Retrieves the data from memory if it has not expired; otherwise, returns default(T).
        public Task<T> GetDataAsync()
        {
            return DateTime.UtcNow - LastFetchTime < Expiration ? Task.FromResult(data) : Task.FromResult(default(T));
        }

        // Asynchronously saves the provided data into memory and updates the last fetch time.

        public Task SetDataAsync(T newData)
        {
            data = newData;
            LastFetchTime = DateTime.UtcNow;
            return Task.CompletedTask;  // Returns a completed task since memory operations are synchronous.
        }
    }


}
