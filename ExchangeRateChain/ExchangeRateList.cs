
namespace ExchangeRateChain
{
    // Represents a list of exchange rates relative to a base currency.
    public class ExchangeRateList
    {
        public int Timestamp { get; set; }
        public string? Base { get; set; }
        public Dictionary<string, decimal>? Rates { get; set; }
    }

}
