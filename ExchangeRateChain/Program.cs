using ExchangeRateChain;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize your storages and ChainResource instance here
        var jsonConverter = new JsonDataConverter<ExchangeRateList>();
        var memoryStorage = new MemoryStorage<ExchangeRateList>();
        var fileSystemStorage = new FileSystemStorage<ExchangeRateList>("./data.json", jsonConverter);
        var webServiceStorage = new WebServiceStorage<ExchangeRateList>("https://openexchangerates.org/api/latest.json?app_id=22e9810ac7694a4da9c7a506f663a4f8", jsonConverter);

        // Create a list of IStorage<ExchangeRateList> and populate it with the initialized storage instances.
        var storages = new List<IReadableStorage<ExchangeRateList>> { memoryStorage, fileSystemStorage, webServiceStorage };
        var chainResource = new ChainResource<ExchangeRateList>(storages);

        // Fetch the data using ChainResource and display it
        Console.WriteLine("First retrieval attempt:");
        var rates = await chainResource.GetValueAsync();
        DisplayRates(rates);

        // Simulate a scenario to test data propagation: Try to retrieve data again, which should now be faster if cached properly.
        Console.WriteLine("\nSecond retrieval attempt after data propagation:");
        rates = await chainResource.GetValueAsync();
        DisplayRates(rates);
    }

    // Displays the exchange rates to the console. Only the top five rates are shown for shortness.
    static void DisplayRates(ExchangeRateList rates)
    {
        if (rates != null)
        {
            // Print out the base currency and up to five exchange rates as a simple demonstration.
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
