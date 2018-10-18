// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth.Internal.Requests
{
    internal class RegisterRequest : BasePostRestRequest<RegisterDto>
    {
        private readonly AuthConfig _authConfig;

        public RegisterRequest(
            IJsonSerializer jsonSerializer, RegisterDto dto, AuthConfig authConfig) : base(jsonSerializer, dto)
        {
            _authConfig = authConfig;
        }

        public override string EndpointUrl => $"{_authConfig.BaseUrl}/api/account/register";

        public override bool UseOriginalEndpoint => true;
    }
}