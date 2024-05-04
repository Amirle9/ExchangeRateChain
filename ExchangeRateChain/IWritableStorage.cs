using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateChain
{
    public interface IWritableStorage<T> : IReadableStorage<T>
    {
        Task SetDataAsync(T data);
    }

}
