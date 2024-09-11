using System.Collections.Concurrent;
//Copyright © 2023 Mandala Consulting, LLC MIT License
//Created by Alexander Fields
namespace Optimization.Repository
{
    public interface IMongoHelperFactory
    {
        IMongoHelper Create(string dbName, string connectionString);
    }

    public class MongoHelperFactory : IMongoHelperFactory
    {
        // Cache to store MongoHelper instances with their respective database and connection strings
        private readonly ConcurrentDictionary<string, IMongoHelper> _cache = new ConcurrentDictionary<string, IMongoHelper>();

        public IMongoHelper Create(string dbName, string connectionString)
        {
            // Combine dbName and connectionString as a cache key
            string cacheKey = $"{connectionString}-{dbName}";

            // Check if the MongoHelper is already cached for the given dbName and connectionString
            if (!_cache.TryGetValue(cacheKey, out IMongoHelper cachedMongoHelper))
            {
                // If not found in cache, create a new instance
                MongoHelper mongoHelper = new MongoHelper();

                mongoHelper.database = mongoHelper.CreateMongoDbInstance(dbName, connectionString);
                mongoHelper.dbName = dbName;
               
                // Cache the newly created MongoHelper
                _cache.TryAdd(cacheKey, mongoHelper);

                return mongoHelper;
            }

            // If cached instance exists, return it
            return cachedMongoHelper;
        }
    }
}
