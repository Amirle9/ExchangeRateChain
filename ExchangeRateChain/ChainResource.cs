namespace ExchangeRateChain
{
    public class ChainResource<T>
    {
        private readonly List<IReadableStorage<T>> storages;

        // Constructor: Initializes a new instance of the ChainResource class with a list of storage providers.
        public ChainResource(List<IReadableStorage<T>> storages)
        {
            this.storages = storages ?? throw new ArgumentNullException(nameof(storages));
        }

        // This method iterates through each storage, checks for data validity,
        // retrieves the data if valid, and propagates it up the chain if necessary.
        public async Task<T> GetValueAsync()
        {
            T value = default(T);       //set the initial value of value based on the type T.

            // Searches through the 'storages' list and finds the first storage that is not read-only, storing it in 'firstWritableStorage'.
            // This is used later to determine where to start propagating data back up the chain.
            var firstWritableStorage = storages.OfType<IWritableStorage<T>>().FirstOrDefault();

            foreach (var storage in storages)
            {
                // Logs details about each storage check
                Console.WriteLine($"Checking storage: {storage.GetType().Name}, ReadOnly: {storage.IsReadOnly}, LastFetchTime: {storage.LastFetchTime}, Expiration: {storage.Expiration}");

                if (!storage.IsReadOnly && DateTime.UtcNow - storage.LastFetchTime >= storage.Expiration)
                {
                    Console.WriteLine("Data is expired, checking next storage.");
                    continue;
                }

                value = await storage.GetDataAsync();                   // Attempt to retrieve data from the current storage.
                if (value != null)
                {
                    // Propagate the data to all writable storages above in the chain
                    if (firstWritableStorage != null && storage != firstWritableStorage)    //checks if there is at least one writable
                                                                                            //storage and that the current storage isn't
                                                                                            //the first writable storage.
                    {

                        // Iterates through the storages until it reaches the current storage 'storage' that is being processed and
                        // is not read-only. This loop is used to propagate data to all earlier writable storages in the chain,
                        // updating them with the latest data fetched.
                        foreach (var writableStorage in storages.OfType<IWritableStorage<T>>().Where(s => storages.IndexOf(s) < storages.IndexOf(storage)))
                        {
                            Console.WriteLine($"Propagating data to {writableStorage.GetType().Name}");
                            await writableStorage.SetDataAsync(value);
                            writableStorage.LastFetchTime = DateTime.UtcNow;
                        }
                    }
                    Console.WriteLine("Data found, breaking loop.");
                    break;
                }
            }

            if (value == null)
            {
                Console.WriteLine("No data found in any storage, returning null.");
            }

            return value;
        }
    }

}
