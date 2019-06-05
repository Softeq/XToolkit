// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.Auth.Internal
{
    internal static class HttpConsts
    {
        public const string GrantTypeKey = "grant_type";
        public const string PasswordKey = "password";
        public const string ClientIdKey = "client_id";
        public const string ClientSecretKey = "client_secret";
        public const string UsernameKey = "username";
        public const string RefreshTokenKey = "refresh_token";
        public const string ScopeKey = "scope";
        public const string ScopeApiKey = "api";
        public const string ScopeOfflineAccessKey = "offline_access";
        public const string ScopeApiOfflineAccessKey = ScopeApiKey + " " + ScopeOfflineAccessKey;
    }
}