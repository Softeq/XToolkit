// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Net.Http;
using Softeq.XToolkit.Auth.Internal.Dtos;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth.Internal.Requests
{
    internal class LoginRequest : BaseRestRequest
    {
        private readonly AuthConfig _config;
        private readonly RegisterDto _dto;

        public LoginRequest(RegisterDto dto, AuthConfig config)
        {
            _dto = dto;
            _config = config;
        }

        public override string EndpointUrl => $"{_config.BaseUrl}/connect/token";

        public override bool UseOriginalEndpoint => true;

        public override bool HasCustomHeaders => true;

        public override IList<(string Header, string Value)> CustomHeaders => new List<(string Header, string Value)>();

        public override HttpMethod Method => HttpMethod.Post;

        public override HttpContent GetContent()
        {
            var dict = new Dictionary<string, string>
            {
                {HttpConsts.GrantTypeKey, HttpConsts.PasswordKey},
                {HttpConsts.ClientIdKey, _config.ClientId},
                {HttpConsts.ClientSecretKey, _config.ClientSecret},
                {HttpConsts.UsernameKey, _dto.Email},
                {HttpConsts.PasswordKey, _dto.Password}
            };

            return new FormUrlEncodedContent(dict);
        }
    }
}