// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.XToolkit.RemoteData.HttpClient
{
    public abstract class BasePagingRestRequest : BaseRestRequest
    {
        private int? _page;
        private int? _pageSize;

        protected BasePagingRestRequest(int? page, int? pageSize)
        {
            _page = page;
            _pageSize = pageSize;
        }

        public sealed override string EndpointUrl
        {
            get
            {
                var result = EndpointUrlMainPart;
                result += _page != null || _pageSize != null ? "?" : string.Empty;
                if (_page != null)
                {
                    result += $"page={_page.Value}";
                    result += _pageSize != null ? "&" : string.Empty;
                }

                if (_pageSize != null)
                {
                    result += $"pageSize={_pageSize.Value}";
                }

                return result;
            }
        }

        protected abstract string EndpointUrlMainPart { get; }
    }
}