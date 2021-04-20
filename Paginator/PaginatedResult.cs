using System.Collections.Generic;

namespace Paginator
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
    }
}
