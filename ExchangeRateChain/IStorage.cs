
namespace ExchangeRateChain
{
    // Defines a generic storage interface for managing data of type T.
    public interface IStorage<T>
    {
        bool IsReadOnly { get; }
        TimeSpan Expiration { get; }
        bool ShouldCheckExpiration { get; }
        DateTime LastFetchTime { get; set; }
        Task<T> GetDataAsync();
        Task SetDataAsync(T data);
    }

}
