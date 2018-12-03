// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softeq.XToolkit.RemoteData
{
    public class QueryStringBuilder
    {
        private readonly IList<string> _urlParams = new List<string>();

        public QueryStringBuilder AddParam<T>(string key, T? value) where T : struct
        {
            if (value.HasValue)
            {
                AddParamImpl(key, value.Value.ToString());
            }

            return this;
        }
    
        public QueryStringBuilder AddParam(string key, string value)
        {
            if (value != null)
            {
                AddParamImpl(key, value);
            }

            return this;
        }
        
        public QueryStringBuilder AddParams(IDictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                AddParamImpl(parameter.Key, parameter.Value);
            }

            return this;
        }

        public string Build()
        {
            return string.Concat(_urlParams.Any() ? "?" : string.Empty, string.Join("&", _urlParams));
        }
        
        private void AddParamImpl(string key, string value)
        {
            _urlParams.Add(string.Concat(HttpUtility.UrlEncode(key), "=", HttpUtility.UrlEncode(value)));
        }
    }
}