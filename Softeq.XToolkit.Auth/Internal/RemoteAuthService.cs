// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.Auth.Internal.Requests;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Common.Logger;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth.Internal
{
    internal class RemoteAuthService
    {
        private readonly AuthConfig _authConfig;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;
        private readonly IRestHttpClient _restHttpClient;

        public RemoteAuthService(
            IJsonSerializer jsonSerializer,
            IRestHttpClient restHttpClient,
            ILogManager logManager,
            AuthConfig authConfig)
        {
            _jsonSerializer = jsonSerializer;
            _restHttpClient = restHttpClient;
            _authConfig = authConfig;
            _logger = logManager.GetLogger<RemoteAuthService>();
        }

        internal Task<LoginResultDto> LogInAsync(string email, string password)
        {
            var dto = new LoginDto
            {
                Email = email,
                Password = password
            };
            var request = new LoginRequest(dto, _authConfig);
            return _restHttpClient.TrySendAndDeserializeAsync<LoginResultDto>(request, _logger);
        }

        internal Task<bool> RegisterAsync(string email, string password, bool isAcceptedTerms)
        {
            var dto = new RegisterDto
            {
                Email = email,
                Password = password,
                IsAcceptedTermsOfService = isAcceptedTerms
            };
            var request = new RegisterRequest(_jsonSerializer, dto, _authConfig);
            return _restHttpClient.TrySendAsync(request, _logger);
        }
    }
}