// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Interfaces;
using Softeq.XToolkit.Common.Logger;

namespace Softeq.XToolkit.RemoteData.HttpClient
{
    public abstract class RestHttpClientBase : IRestHttpClient
    {
        private readonly IRefreshTokenService _refreshTokenService;
        protected readonly IJsonSerializer JsonSerializer;
        protected readonly ILogger Logger;

        protected RestHttpClientBase(
            IJsonSerializer jsonSerializer,
            IRefreshTokenService refreshTokenService,
            ILogManager logManager)
        {
            JsonSerializer = jsonSerializer;
            _refreshTokenService = refreshTokenService;
            Logger = logManager.GetLogger(GetType().ToString());
        }

        public abstract string ApiUrl { get; }

        protected abstract bool IsAuthorized { get; }

        protected abstract string AccessToken { get; }

        public virtual async Task<string> SendAndGetResponseAsync(BaseRestRequest request)
        {
            var response = await SendAsyncImpl(request).ConfigureAwait(false);
            if (response == null)
            {
                return null;
            }

            var stringResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

#if DEBUG
            Logger.Debug(stringResponse);
#endif

            response.Dispose();
            return stringResponse;
        }

        public async Task<T> SendAndDeserializeAsync<T>(BaseRestRequest request)
        {
            var stringResponse = await SendAndGetResponseAsync(request).ConfigureAwait(false);
            if (stringResponse == null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(stringResponse);
        }

        public async Task<bool> SendAsync(BaseRestRequest request)
        {
            var result = default(bool);

            var response = await SendAsyncImpl(request).ConfigureAwait(false);
            if (response != null)
            {
                result = response.IsSuccessStatusCode;
                response.Dispose();
            }

            return result;
        }

        public async Task<Stream> GetStreamAsync(BaseRestRequest request)
        {
            var response = await SendAsyncImpl(request).ConfigureAwait(false);
            if (response == null)
            {
                return null;
            }

            var result = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            response.Dispose();
            return result;
        }

        protected virtual ServerException CreateException(string responseString, HttpStatusCode statusCode)
        {
            if (string.IsNullOrEmpty(responseString))
            {
                return new ServerException("Unknown server error") { StatusCode = statusCode };
            }

            Logger.Debug(responseString);

            //try parse response 
            var errorDto = default(ErrorDto);
            try
            {
                errorDto = JsonSerializer.Deserialize<ErrorDto>(responseString);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }

            if (errorDto != null)
            {
                return new ServerException("Server error", new[] { ToServerError(errorDto) }) { StatusCode = statusCode };
            }

            var errors = default(List<ErrorDescription>);
            try
            {
                var errorDtos = JsonSerializer.Deserialize<List<ErrorDto>>(responseString);
                errors = errorDtos.Select(ToServerError).ToList();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
            }

            var exception = errors == null
                ? new ServerException("Unknown server error") { StatusCode = statusCode }
                : new ServerException("Server error", errors) { StatusCode = statusCode };

            TryHandleException(exception);

            return exception;
        }

        protected virtual void TryHandleException(ServerException serverException)
        {
            // do nothing
        }

        protected virtual async Task<HttpResponseMessage> SendAsyncImpl(BaseRestRequest request)
        {
            using (var httpClientHandler = GetHandler())
            {
                if (httpClientHandler.SupportsAutomaticDecompression)
                {
                    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
                }

                using (var httpClient = new System.Net.Http.HttpClient(httpClientHandler))
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
                        }

                        response = await httpClient.SendAsync(httpRequest).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    response = await HandleErrors(response, request).ConfigureAwait(false);

                    return response;
                }
            }
        }

        protected HttpClientHandler GetHandler()
        {
#if DEBUG
            return new HttpDiagnosticsHandler(Logger);
#else
            return new HttpClientHandler();
#endif
        }

        protected async Task<HttpResponseMessage> HandleErrors(HttpResponseMessage response, BaseRestRequest request)
        {
            if (response == null || response.IsSuccessStatusCode)
            {
                return response;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (IsAuthorized)
                {
                    if (await _refreshTokenService.RefreshToken(this).ConfigureAwait(false))
                    {
                        return await SendAsyncImpl(request).ConfigureAwait(false);
                    }
                }
            }

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.Dispose();

            throw CreateException(responseString, response.StatusCode);
        }

        protected Uri GetFullUrl(BaseRestRequest request)
        {
            return request.UseOriginalEndpoint
                ? new Uri(request.EndpointUrl)
                : new Uri($"{ApiUrl}{request.EndpointUrl}");
        }

        private static ErrorDescription ToServerError(ErrorDto x)
        {
            return new ErrorDescription
            {
                Code = x.Code,
                Description = x.Description,
                DetailedErrorCode = x.DetailedError
            };
        }
    }
}