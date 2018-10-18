// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.Common.Extensions;
using Softeq.XToolkit.Common.Interfaces;

namespace Softeq.XToolkit.Auth
{
    public class AccountService
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

        private AccountInfo AccountInfo =>
            _accountInfo ?? (_accountInfo = GetAccountInfoFromInternalSettings() ?? new AccountInfo());

        public bool IsAuthorized => !string.IsNullOrEmpty(AccountInfo.AccessToken);

        public string AccessToken => AccountInfo.AccessToken;

        public string RefreshToken => AccountInfo.RefreshToken;

        public string Email => AccountInfo.Email;

        internal void Clear()
        {
            _accountInfo = null;
            SetAccountInfoToInternalSettings(default(AccountInfo));
        }

        internal void ResetTokens(LoginResultDto loginResult)
        {
            AccountInfo.AccessToken = loginResult.AccessToken;
            AccountInfo.RefreshToken = loginResult.RefreshToken;
            SetAccountInfoToInternalSettings(AccountInfo);
        }

        internal void SaveProfileInfo(string email)
        {
            AccountInfo.Email = email;
            SetAccountInfoToInternalSettings(AccountInfo);
        }

        private AccountInfo GetAccountInfoFromInternalSettings()
        {
            return _internalSettings.GetJsonValueOrDefault(_jsonSerializer, nameof(AccountInfo), default(AccountInfo));
        }

        private void SetAccountInfoToInternalSettings(AccountInfo accountInfo)
        {
            _internalSettings.AddOrUpdateJsonValue(_jsonSerializer, nameof(AccountInfo), accountInfo);
        }
    }
}