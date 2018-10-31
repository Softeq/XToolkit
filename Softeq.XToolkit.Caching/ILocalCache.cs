// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;

namespace Softeq.XToolkit.Caching
{
    public interface ILocalCache
    {
        void Initialize();

        Task<DateTimeOffset?> GetTimeStampForKey(string key);

        Task<string> GetFromCache(string key);

        Task SaveToCache(string key, DateTimeOffset date, string json);
    }
}