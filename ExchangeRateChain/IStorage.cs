
namespace ExchangeRateChain
{
    // Defines a generic storage interface for managing data of type T.
    public interface IStorage<T>
    {
        bool IsReadOnly { get; }
        TimeSpan Expiration { get; }
        DateTime LastFetchTime { get; set; }   // Property to track the last time data was fetched or updated.

        Task<T> GetDataAsync(); // Method to asynchronously retrieve data of type T from the storage.

        Task SetDataAsync(T data); // Method to asynchronously store data of type T into the storage.

    }

}
