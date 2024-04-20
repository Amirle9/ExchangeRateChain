namespace ExchangeRateChain
{
    public class ChainResource<T>
    {
        private readonly List<IStorage<T>> storages;

        public ChainResource(List<IStorage<T>> storages)
        {
            this.storages = storages ?? throw new ArgumentNullException(nameof(storages));
        }

        // This method iterates through each storage, checks for data validity, retrieves the data if valid, and propagates it up the chain if necessary.
        public async Task<T> GetValueAsync()
        {
            T value = default(T);
            IStorage<T> firstWritableStorage = storages.FirstOrDefault(s => !s.IsReadOnly);

            foreach (var storage in storages)
            {
                Console.WriteLine($"Checking storage: {storage.GetType().Name}, ReadOnly: {storage.IsReadOnly}, LastFetchTime: {storage.LastFetchTime}, Expiration: {storage.Expiration}");

                if (storage.ShouldCheckExpiration && DateTime.Now - storage.LastFetchTime >= storage.Expiration)
                {
                    Console.WriteLine("Data is expired, checking next storage.");
                    continue;
                }

                value = await storage.GetDataAsync();                   // Attempt to retrieve data from the current storage.
                if (value != null)
                {
                    // Propagate the data to all writable storages above in the chain
                    if (firstWritableStorage != null && storage != firstWritableStorage)
                    {
                        foreach (var writableStorage in storages.TakeWhile(s => s != storage && !s.IsReadOnly))
                        {
                            Console.WriteLine($"Propagating data to {writableStorage.GetType().Name}");
                            await writableStorage.SetDataAsync(value);
                            writableStorage.LastFetchTime = DateTime.Now;   // Update the last fetch time post propagation.
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
