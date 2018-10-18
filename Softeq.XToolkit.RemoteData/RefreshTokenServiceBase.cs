// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading;
using System.Threading.Tasks;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.RemoteData
{
    public enum RefreshTokenStatus
    {
        CanBeRefreshed,
        Refreshed,
        Error
    }

    public abstract class RefreshTokenServiceBase : IRefreshTokenService
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        private long _entries;
        private RefreshTokenStatus _refreshTokenStatus;

        protected RefreshTokenServiceBase()
        {
            _semaphoreSlim = new SemaphoreSlim(1);
            _refreshTokenStatus = RefreshTokenStatus.CanBeRefreshed;
        }

        public async Task<bool> RefreshToken(IRestHttpClient restHttpClient)
        {
            if (_refreshTokenStatus == RefreshTokenStatus.Refreshed)
            {
                return true;
            }

            Interlocked.Increment(ref _entries);

            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

            if (_refreshTokenStatus == RefreshTokenStatus.CanBeRefreshed)
            {
                _refreshTokenStatus = await TryRefreshToken(restHttpClient).ConfigureAwait(false);
            }

            _semaphoreSlim.Release();

            Interlocked.Decrement(ref _entries);
            if (Interlocked.Read(ref _entries) == 0 && _refreshTokenStatus != RefreshTokenStatus.Error)
            {
                _refreshTokenStatus = RefreshTokenStatus.CanBeRefreshed;
            }

            return _refreshTokenStatus != RefreshTokenStatus.Error;
        }

        protected abstract Task<RefreshTokenStatus> TryRefreshToken(IRestHttpClient restHttpClient);
    }
}