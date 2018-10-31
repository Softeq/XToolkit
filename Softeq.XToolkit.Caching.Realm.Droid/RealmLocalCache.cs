// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Globalization;
using System.Threading.Tasks;
using RealmType = Realms.Realm;

namespace Softeq.XToolkit.Caching.Realm.Droid
{
    public class RealmLocalCache : ILocalCache
    {
        public Task<string> GetFromCache(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var realm = RealmType.GetInstance())
                {
                    var result = realm.Find<LocalData>(key);
                    return result.Data;
                }
            });
        }

        public Task<DateTimeOffset?> GetTimeStampForKey(string key)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var realm = RealmType.GetInstance())
                {
                    var result = realm.Find<LocalData>(key);
                    if (result == null)
                    {
                        return default(DateTimeOffset?);
                    }

                    return DateTimeOffset.Parse(result.Timestamp, CultureInfo.InvariantCulture) as DateTimeOffset?;
                }
            });
        }

        public void Initialize()
        {
        }

        public Task SaveToCache(string key, DateTimeOffset date, string json)
        {
            var tcs = new TaskCompletionSource<bool>();

            var localData = new LocalData {Data = json, Timestamp = date.ToString("r"), Key = key};

            using (var realm = RealmType.GetInstance())
            {
                realm.Write(() =>
                {
                    realm.Add(localData, true);
                    tcs.TrySetResult(true);
                });
            }

            return tcs.Task;
        }
    }
}