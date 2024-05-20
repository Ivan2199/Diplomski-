using System;
using System.Collections.Generic;

namespace GeoTagMap.Common
{
    public class PagingInfo<T>
    {
        public List<T> List { get; set; }
        public int RRP { get; set; }
        public int PageNumber { get; set; }
        public int TotalSize { get; set; }
    }
}
