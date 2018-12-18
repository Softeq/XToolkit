// Developed by Softeq Development Corporation
// http://www.softeq.com

using Realms;

namespace Softeq.XToolkit.Caching.Realm
{
    internal class LocalData : RealmObject
    {
        [PrimaryKey]
        public string Key { get; set; }

        public string Timestamp { get; set; }

        public string Data { get; set; }
    }
}