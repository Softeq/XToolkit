// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.Auth.Internal.Requests;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth
{
    public class RefreshTokenService : RefreshTokenServiceBase
    {
        private readonly IAccountService _accountService;
        private readonly AuthConfig _authConfig;
        private readonly ILogger _logger;

        public RefreshTokenService(
            ILogManager logManager,
            IAccountService accountService,
            AuthConfig authConfig)
        {
            _logger = logManager.GetLogger<RefreshTokenService>();
            _accountService = accountService;
            _authConfig = authConfig;
        }

        protected override async Task<RefreshTokenStatus> TryRefreshToken(IRestHttpClient restHttpClient)
        {
            var attemptCount = 3;
            var result = RefreshTokenStatus.Error;
            var loginResult = default(LoginResultDto);

            while (attemptCount > 0 && result != RefreshTokenStatus.Refreshed)
            {
                try
                {
                    _logger.Info("Try refresh access token...");
                    var request = new RefreshTokenRequest(_accountService.RefreshToken, _authConfig);
                    loginResult = await restHttpClient
                        .TrySendAndDeserializeAsync<LoginResultDto>(request, _logger)
                        .ConfigureAwait(false);
                    if (loginResult != null)
                    {
                        result = RefreshTokenStatus.Refreshed;
                    }

                    _logger.Info("Access token refreshed");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
                finally
                {
                    attemptCount--;
                }
            }

            if (loginResult != null)
            {
                try
                {
                    _accountService.ResetTokens(loginResult.AccessToken, loginResult.RefreshToken);
                    _logger.Info("Access token saved");
                }
                catch (Exception)
                {
                    _logger.Error("Access token is not saved");
                }
            }

            if (result == RefreshTokenStatus.Error)
            {
                //TODO: logout
            }

            return result;
        }
    }
}