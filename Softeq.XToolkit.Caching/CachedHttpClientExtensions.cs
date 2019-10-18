// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Logger;
using Softeq.XToolkit.RemoteData;
using Softeq.XToolkit.RemoteData.HttpClient;

namespace Softeq.XToolkit.Caching
{
    public static class CachedHttpClientExtensions
    {
        public static async Task<TModel> GetModelAsync<TModel, TDto>(
            this IRestHttpClient restHttpClient,
            BaseRestRequest request,
            ILogger logger,
            Func<TDto, TModel> dtoToModelConverter,
            bool ignoreCache)
        {
            var result =
                await restHttpClient.GetModelOrExceptionAsync(request, logger, dtoToModelConverter, ignoreCache);
            return result.Model;
        }

        public static async Task<(TModel Model, ServerException Exception)> GetModelOrExceptionAsync<TModel, TDto>(
            this IRestHttpClient restHttpClient,
            BaseRestRequest request,
            ILogger logger,
            Func<TDto, TModel> dtoToModelConverter,
            bool ignoreCache)
        {
            try
            {
                if (!(restHttpClient is CachedHttpClient client))
                {
                    throw new Exception("You should use CachedHttpClient");
                }

                var dto = await client.SendAndDeserializeAsync<TDto>(request, ignoreCache).ConfigureAwait(false);
                var model = dtoToModelConverter(dto);
                return (model, null);
            }
            catch (ServerException e)
            {
                return (default(TModel), e);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return (default(TModel), null);
        }
    }
}