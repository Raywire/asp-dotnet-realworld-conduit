namespace Conduit.ResourceParameters
{
    public class ArticlesResourceParameters
    {
        const int maxPageSize = 20;
        public string Author { get; set; }
        public string Search { get; set; }
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}
