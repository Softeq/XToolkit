// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.RemoteData
{
    public interface IRefreshTokenService
    {
        Task<bool> RefreshToken(IRestHttpClient restHttpClient);
    }
}