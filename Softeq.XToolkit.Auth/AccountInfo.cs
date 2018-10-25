// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.Auth
{
    internal class AccountInfo
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string Email { get; set; }

        public string UserId { get; set; }

        public string UserDisplayName { get; set; }

        public string UserPhotoUrl { get; set; }

        public bool IsProxyEnabled { get; set; }

        public string ProxyAddress { get; set; }
    }
}