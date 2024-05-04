
namespace ExchangeRateChain
{
    public interface IDataConverter<T>
    {
        string Serialize(T data);
        T Deserialize(string data);
    }
}
