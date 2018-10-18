// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Net.Http;

namespace Softeq.XToolkit.RemoteData.HttpClient
{
    public abstract class BaseRestRequest
    {
        public abstract string EndpointUrl { get; }

        public virtual HttpMethod Method => HttpMethod.Get;

        public virtual bool UseOriginalEndpoint => false;

        public virtual IList<(string Header, string Value)> CustomHeaders =>
            default(IList<(string Header, string Value)>);

        public virtual bool HasCustomHeaders => CustomHeaders != null && CustomHeaders.Count > 0;

        public virtual HttpContent GetContent()
        {
            return null;
        }
    }
}