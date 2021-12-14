using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace GameNight.Models.CacheUtils
{
    public static class CacheUtils
    {

        public static IEnumerable<TItem> GetAllCacheItems<TItem>(this IMemoryCache cache)
        {
            var value = cache.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(cache);
            var collection = value as ICollection;
            var items = new List<string>();
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
            }

            var cachedItems = new List<TItem>();

            Parallel.ForEach(items, (item) =>
            {
                cachedItems.Add(cache.Get<TItem>(item));
            });

            return cachedItems;
        }
    }
}
