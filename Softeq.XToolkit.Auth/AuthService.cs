// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.XToolkit.Auth.Internal;
using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAccountService _accountService;
        private readonly RemoteAuthService _remoteAuthService;

        public AuthService(
            IJsonSerializer jsonSerializer,
            IRestHttpClient restHttpClient,
            ILogManager logManager,
            IAccountService accountService,
            AuthConfig authConfig)
        {
            _remoteAuthService = new RemoteAuthService(jsonSerializer, restHttpClient, logManager, authConfig);
            _accountService = accountService;
        }

        public async Task<bool> RegisterAsync(string email, string password, bool isAcceptedTerms)
        {
            var result = await _remoteAuthService.RegisterAsync(email, password, isAcceptedTerms).ConfigureAwait(false);
            if (!result)
            {
                return false;
            }

            return await LoginAsync(email, password).ConfigureAwait(false);
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var loginResult = await _remoteAuthService.LogInAsync(email, password).ConfigureAwait(false);
            if (loginResult == null)
            {
                return false;
            }

            CompleteLoginAsync(loginResult, email);

            return true;
        }

        public void Logout()
        {
            _accountService.Logout();
        }

        private void CompleteLoginAsync(LoginResultDto loginResult, string email)
        {
            //clear all stored data about user
            _accountService.Logout();

            //save auth tokens
            _accountService.ResetTokens(loginResult.AccessToken, loginResult.RefreshToken);
            _accountService.SaveProfileInfo(email, null, null);
        }
    }
}