// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Auth
{
    public class AccountService : IAccountService
    {
        private readonly IInternalSettings _internalSettings;
        private readonly IJsonSerializer _jsonSerializer;

        private AccountInfo _accountInfo;

        public AccountService(
            IInternalSettings internalSettings,
            IJsonSerializer jsonSerializer)
        {
            _internalSettings = internalSettings;
            _jsonSerializer = jsonSerializer;
        }

        public string UserId => AccountInfo.UserId;

        public string UserName => AccountInfo.UserDisplayName;

        public string UserPhotoUrl => AccountInfo.UserPhotoUrl;

        public bool IsAuthorized => !string.IsNullOrEmpty(AccountInfo.AccessToken);

        public string AccessToken => AccountInfo.AccessToken;

        public bool IsProxyEnabled => AccountInfo.IsProxyEnabled;

        public string ProxyAddress => AccountInfo.ProxyAddress;

        public string RefreshToken => AccountInfo.RefreshToken;

        private AccountInfo AccountInfo =>
            _accountInfo ?? (_accountInfo = AccountInfoSetting ?? new AccountInfo());

        public void Logout()
        {
            _accountInfo = null;
            AccountInfoSetting = _accountInfo;
        }

        public void ResetTokens(string accessToken, string refreshToken)
        {
            AccountInfo.AccessToken = accessToken;
            AccountInfo.RefreshToken = refreshToken;
            AccountInfoSetting = AccountInfo;
        }

        public void SaveProfileInfo(string modelUserId, string modelName, string modelThumbnailUrl)
        {
            AccountInfo.UserId = modelUserId;
            AccountInfo.UserDisplayName = modelName;
            AccountInfo.UserPhotoUrl = modelThumbnailUrl;
            AccountInfoSetting = AccountInfo;
        }

        public void SaveProxySettings(string proxyAddress, bool isProxyEnabled)
        {
            AccountInfo.IsProxyEnabled = isProxyEnabled;
            AccountInfo.ProxyAddress = proxyAddress;
            AccountInfoSetting = AccountInfo;
        }

        private readonly string _accountInfoKey = $"{nameof(AccountService)}_{nameof(AccountInfo)}";
        private AccountInfo AccountInfoSetting
        {
            get => _internalSettings.GetJsonValueOrDefault(_jsonSerializer, _accountInfoKey, default(AccountInfo));
            set => _internalSettings.AddOrUpdateJsonValue(_jsonSerializer, _accountInfoKey, value);
        }
    }
}