// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.Auth
{
    public interface IAccountService
    {
        bool IsProxyEnabled { get; }
        string ProxyAddress { get; }
        string UserId { get; }
        string UserName { get; }
        string UserPhotoUrl { get; }
        bool IsAuthorized { get; }
        string AccessToken { get; }
        string RefreshToken { get; }

        void SaveProxySettings(string proxyAddress, bool isProxyEnabled);
        void ResetTokens(string accessToken, string refreshToken);
        void SaveProfileInfo(string modelUserId, string modelName, string modelThumbnailUrl);
        void Logout();
    }
}