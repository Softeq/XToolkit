// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Net.Http;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Auth.Internal.Requests
{
    internal class RefreshTokenRequest : BaseRestRequest
    {
        private readonly AuthConfig _authConfig;
        private readonly string _refreshToken;

        public RefreshTokenRequest(string refreshToken, AuthConfig authConfig)
        {
            _refreshToken = refreshToken;
            _authConfig = authConfig;
        }

        public override string EndpointUrl => $"{_authConfig.BaseUrl}/connect/token";

        public override bool UseOriginalEndpoint => true;

        public override bool HasCustomHeaders => true;

        public override IList<(string Header, string Value)> CustomHeaders => new List<(string Header, string Value)>();

        public override HttpMethod Method => HttpMethod.Post;

        public override HttpContent GetContent()
        {
            var dict = new Dictionary<string, string>
            {
                {HttpConsts.ClientIdKey, _authConfig.ClientId},
                {HttpConsts.ClientSecretKey, _authConfig.ClientSecret},
                {HttpConsts.RefreshTokenKey, _refreshToken},
                {HttpConsts.GrantTypeKey, HttpConsts.RefreshTokenKey}
            };

            return new FormUrlEncodedContent(dict);
        }
    }
}