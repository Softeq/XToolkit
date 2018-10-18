// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Softeq.XToolkit.RemoteData
{
    public class PagingModelDto<T>
    {
        public int PageNumber { get; set; }
        public int TotalNumberOfPages { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int PageSize { get; set; }

        [JsonProperty("results")] public IList<T> Items { get; set; }
    }
}