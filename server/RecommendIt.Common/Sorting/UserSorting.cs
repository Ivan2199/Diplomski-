using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Common.Sorting
{
    public class UserSorting
    {
        public string OrderBy { get; set; }
        public string SortOrdering { get; set; }

        public UserSorting(string orderby, string sortordering)
        { OrderBy = orderby; SortOrdering = sortordering; }
    }
}
