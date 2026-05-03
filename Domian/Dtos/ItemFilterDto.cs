using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Dtos
{
    public class ItemFilterDto
    {
        public List<string> Brands { get; set; } = new();        // CategoryName
        public List<string> LaptopTypes { get; set; } = new();  // ItemTypeName
        public List<string> OS { get; set; } = new();           // OsName
        public List<string> Processors { get; set; } = new();   // Processor
        public List<int> Rams { get; set; } = new();             // RamSize

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
