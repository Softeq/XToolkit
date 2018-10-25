// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Net.Http;
using System.Text;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.RemoteData.HttpClient
{
    public abstract class BasePostRestRequest<T> : BaseRestRequest
    {
        private readonly IJsonSerializer _jsonSerializer;

        protected readonly T Dto;
        
        protected BasePostRestRequest(IJsonSerializer jsonSerializer, T dto)
        {
            Dto = dto;
            _jsonSerializer = jsonSerializer;
        }

        public override HttpMethod Method => HttpMethod.Post;

        public override HttpContent GetContent()
        {
            var content = _jsonSerializer.Serialize(Dto);
            return new StringContent(content, Encoding.UTF8, HttpConsts.ApplicationJsonHeaderValue);
        }
    }
}