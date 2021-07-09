using System;
namespace Conduit.ResourceParameters
{
    public class ResourceParameters
    {
        const int maxPageSize = 20;
        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value <= 0) ? 1 : value;
        }
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
