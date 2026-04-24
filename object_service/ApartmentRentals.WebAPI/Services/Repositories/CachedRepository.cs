using ApartmentRentals.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ApartmentRentals.WebAPI.Services.Repositories
{
    public class CachedRepository<T> : IRepository<T> where T : IModel
    {
        private IRepository<T> Repository { get; set; }
        private IDistributedCache Cache { get; set; }

        private readonly string CACHE_NAMESPACE = typeof(T).Name;
        private const int ABSOLUTE_CACHED_TIME = 2;
        private const int SLIDING_CACHED_TIME = 1;

        public CachedRepository(IRepository<T> repository, IDistributedCache cache)
        {
            Repository = repository;
            Cache = cache;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<T> objects;
            var cacheKey = $"{CACHE_NAMESPACE}:all";

            var cachedData = await Cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                objects = JsonSerializer.Deserialize<List<T>>(cachedData) ?? new List<T>();
            }
            else
            {
                objects = await Repository.GetAllAsync();

                if (objects.Count() != 0)
                {
                    var serializedData = JsonSerializer.Serialize(objects);
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(ABSOLUTE_CACHED_TIME));
                    await Cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                }
            }

            return objects;
        }

        public async Task<IEnumerable<T>> GetFilteredByPropertyAsync(string propertyName, string value)
        {
            IEnumerable<T> objects;
            var cacheKey = $"{CACHE_NAMESPACE}:filter:{propertyName}:{value}";

            var cachedData = await Cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                objects = JsonSerializer.Deserialize<List<T>>(cachedData) ?? new List<T>();
            }
            else
            {
                objects = await Repository.GetFilteredByPropertyAsync(propertyName, value);

                if (objects.Count() != 0)
                {
                    var serializedData = JsonSerializer.Serialize(objects);
                    var cacheOptions = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(ABSOLUTE_CACHED_TIME));
                    await Cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                }
            }

            return objects;
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            T? _object;
            var cacheKey = $"{CACHE_NAMESPACE}:id:{id}";

            var cachedData = await Cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _object = JsonSerializer.Deserialize<T>(cachedData);
            }
            else
            {
                _object = await Repository.GetByIdAsync(id);

                if (_object != null)
                {
                    var serializedData = JsonSerializer.Serialize(_object);
                    var cacheOptions = new DistributedCacheEntryOptions()
                       // .SetAbsoluteExpiration(TimeSpan.FromMinutes(ABSOLUTE_CACHED_TIME))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(SLIDING_CACHED_TIME));
                    await Cache.SetStringAsync(cacheKey, serializedData, cacheOptions);
                }
            }

            return _object;
        }

        public async Task CreateAsync(T newSpace) => await Repository.CreateAsync(newSpace);

        public async Task<bool> UpdateAsync(string id, T updatedSpace)
        {
            var success = await Repository.UpdateAsync(id, updatedSpace);
            if (success)
            {
                var cacheKey = $"{CACHE_NAMESPACE}:id:{id}";
                await Cache.RemoveAsync(cacheKey);
            }

            return success;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            var success = await Repository.DeleteByIdAsync(id);
            if (success)
            {
                var cacheKey = $"{CACHE_NAMESPACE}:id:{id}";
                await Cache.RemoveAsync(cacheKey);
            }

            return success;
        }

        public async Task<bool> DeleteAllAsync() => await Repository.DeleteAllAsync();
    }
}
