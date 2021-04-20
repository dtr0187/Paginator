using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paginator
{
    public class Pagination
    {
        public Pagination()
        {
            Page = 1;
            PageSize = 0;
        }
        public Pagination(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }


        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
