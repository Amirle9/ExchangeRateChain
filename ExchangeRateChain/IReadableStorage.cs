using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateChain
{
    public interface IReadableStorage<T>
    {
        bool IsReadOnly { get; }
        TimeSpan Expiration { get; }
        DateTime LastFetchTime { get; set; }
        Task<T> GetDataAsync();
    }
}
