using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateChain
{
    public class JsonDataConverter<T> : IDataConverter<T>
    {
        public string Serialize(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
