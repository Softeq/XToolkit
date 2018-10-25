// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;
using System.Threading.Tasks;

namespace Softeq.XToolkit.RemoteData.HttpClient
{
    public interface IRestHttpClient
    {
        Task<string> SendAndGetResponseAsync(BaseRestRequest request);
        Task<T> SendAndDeserializeAsync<T>(BaseRestRequest request);
        Task<bool> SendAsync(BaseRestRequest request);
        Task<Stream> GetStreamAsync(BaseRestRequest request);
        string ApiUrl { get; }
    }
}