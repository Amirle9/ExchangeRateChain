using ExchangeRateChain;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize your storages and ChainResource instance here
        var memoryStorage = new MemoryStorage<ExchangeRateList>();
        var fileSystemStorage = new FileSystemStorage<ExchangeRateList>();
        var webServiceStorage = new WebServiceStorage<ExchangeRateList>();

        var storages = new List<IStorage<ExchangeRateList>> { memoryStorage, fileSystemStorage, webServiceStorage };
        var chainResource = new ChainResource<ExchangeRateList>(storages);

        // Fetch the data using ChainResource and display it
        Console.WriteLine("First retrieval attempt:");
        var rates = await chainResource.GetValueAsync();
        DisplayRates(rates);

        // Simulate time passage and try to fetch again to see if data is fetched from updated storage
        Console.WriteLine("\nSecond retrieval attempt after data propagation:");
        rates = await chainResource.GetValueAsync();
        DisplayRates(rates);
    }

    // Displays the exchange rates to the console. Only the top five rates are shown for shortness.
    static void DisplayRates(ExchangeRateList rates)
    {
        if (rates != null)
        {
            Console.WriteLine("Exchange Rates Retrieved:");
            Console.WriteLine($"Base Currency: {rates.Base}");
            foreach (var rate in rates.Rates.Take(5))
            {
                Console.WriteLine($"1 {rates.Base} = {rate.Value} {rate.Key}");
            }
        }
        else
        {
            Console.WriteLine("Failed to retrieve exchange rates.");
        }
    }
}
