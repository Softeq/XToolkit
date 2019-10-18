// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Common.Logger;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Caching
{
    public abstract class CachedHttpClient : RestHttpClientBase
    {
        private const string LastModifiedHeaderKey = "Last-Modified";

        private readonly ICache _localCache;

        protected CachedHttpClient(
            IJsonSerializer jsonSerializer,
            IRefreshTokenService refreshTokenService,
            ILogManager logManager,
            ICache localCache) : base(jsonSerializer, refreshTokenService, logManager)
        {
            _localCache = localCache;
        }

        public override Task<string> SendAndGetResponseAsync(BaseRestRequest request)
        {
            return SendAndGetResponseAsync(request, false);
        }

        public async Task<string> SendAndGetResponseAsync(BaseRestRequest request, bool ignoreCache)
        {
            var response = await SendAsyncImpl(request, ignoreCache).ConfigureAwait(false);
            if (response == null)
            {
                return null;
            }

            if (!ignoreCache && response.StatusCode == HttpStatusCode.NotModified)
            {
                return await _localCache.Get<string>(request.EndpointUrl).ConfigureAwait(false);
            }

            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!ignoreCache)
            {
                var lastModified = GetLastModifiedHeader(response.Content.Headers);
                if (lastModified.HasValue)
                {
                    await _localCache
                        .Add(request.EndpointUrl, lastModified.Value, stringResponse)
                        .ConfigureAwait(false);
                }
            }

#if DEBUG
            Logger.Debug(stringResponse);
#endif

            response.Dispose();
            return stringResponse;
        }

        internal async Task<T> SendAndDeserializeAsync<T>(BaseRestRequest request, bool ignoreCache)
        {
            var stringResponse = await SendAndGetResponseAsync(request, ignoreCache).ConfigureAwait(false);
            if (stringResponse == null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(stringResponse);
        }

        protected override Task<HttpResponseMessage> SendAsyncImpl(BaseRestRequest request)
        {
            return SendAsyncImpl(request, false);
        }

        protected async Task<HttpResponseMessage> SendAsyncImpl(BaseRestRequest request, bool ignoreCache)
        {
            using (var httpClientHandler = GetHandler())
            {
                if (httpClientHandler.SupportsAutomaticDecompression)
                {
                    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
                }

                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(HttpConsts.ApplicationJsonHeaderValue));
                    httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };

                    HttpResponseMessage response = null;

                    try
                    {
                        var httpRequest = new HttpRequestMessage
                        {
                            Method = request.Method,
                            RequestUri = GetFullUrl(request)
                        };

                        if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                        {
                            httpRequest.Content = request.GetContent();
                        }

                        if (request.HasCustomHeaders)
                        {
                            foreach (var header in request.CustomHeaders)
                            {
                                httpRequest.Headers.Add(header.Header, header.Value);
                            }
                        }
                        else
                        {
                            if (IsAuthorized)
                            {
                                httpClient.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue(HttpConsts.Bearer, AccessToken);
                            }

                            if (request.Method == HttpMethod.Get && !ignoreCache)
                            {
                                var timeStamp = await _localCache
                                    .GetExpiration(request.EndpointUrl)
                                    .ConfigureAwait(false);

                                httpRequest.Headers.IfModifiedSince = timeStamp;
                            }
                        }

                        response = await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    if (response != null && response.StatusCode == HttpStatusCode.NotModified)
                    {
                        return response;
                    }

                    response = await HandleErrors(response, request).ConfigureAwait(false);

                    return response;
                }
            }
        }

        private static DateTimeOffset? GetLastModifiedHeader(HttpContentHeaders keyValuePairs)
        {
            foreach (var header in keyValuePairs)
            {
                if (header.Key == LastModifiedHeaderKey)
                {
                    return DateTimeOffset.Parse(header.Value.FirstOrDefault(), CultureInfo.InvariantCulture);
                }
            }

            return null;
        }
    }
}