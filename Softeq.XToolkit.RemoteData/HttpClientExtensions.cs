// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Softeq.XToolkit.Common.Logger;
using Softeq.XToolkit.RemoteData.HttpClient;
using Softeq.XToolkit.WhiteLabel.Model;

namespace Softeq.XToolkit.RemoteData
{
    public static class HttpClientExtensions
    {
        public static async Task<TModel> GetModelAsync<TModel, TDto>(
            this IRestHttpClient restHttpClient,
            BaseRestRequest request,
            ILogger logger,
            Func<TDto, TModel> dtoToModelConverter)
        {
            var result = await restHttpClient.GetModelOrExceptionAsync(request, logger, dtoToModelConverter);
            return result.Model;
        }

        public static async Task<(TModel Model, ServerException Exception)> GetModelOrExceptionAsync<TModel, TDto>(
            this IRestHttpClient restHttpClient,
            BaseRestRequest request,
            ILogger logger,
            Func<TDto, TModel> dtoToModelConverter)
        {
            try
            {
                var dto = await restHttpClient.SendAndDeserializeAsync<TDto>(request).ConfigureAwait(false);
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

        public static async Task<PagingModel<TModel>> GetPagingModelAsync<TModel, TDto>(
            this IRestHttpClient restHttpClient,
            BaseRestRequest request,
            ILogger logger,
            Func<TDto, TModel> dtoToModelConverter)
        {
            try
            {
                var dto = await restHttpClient.SendAndDeserializeAsync<PagingModelDto<TDto>>(request)
                    .ConfigureAwait(false);
                var model = dto == null
                    ? null
                    : new PagingModel<TModel>
                    {
                        Page = dto.PageNumber,
                        Data = dto.Items.Select(dtoToModelConverter).ToList(),
                        TotalNumberOfPages = dto.TotalNumberOfPages,
                        TotalNumberOfRecords = dto.TotalNumberOfRecords,
                        PageSize = dto.PageSize
                    };
                return model;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return default(PagingModel<TModel>);
        }

        public static async Task<T> TrySendAndDeserializeAsync<T>(this IRestHttpClient restHttpClient,
            BaseRestRequest request, ILogger logger)
        {
            try
            {
                var result = await restHttpClient.SendAndDeserializeAsync<T>(request).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return default(T);
        }

        public static async Task<bool> TrySendAsync(this IRestHttpClient restHttpClient, BaseRestRequest request,
            ILogger logger)
        {
            try
            {
                return await restHttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }
    }
}