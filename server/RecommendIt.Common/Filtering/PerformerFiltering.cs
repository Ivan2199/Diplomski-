using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Common.Filtering
{
    public class PerformerFiltering
    {
        public string Name { get; set; }
        public string BandOrMusician { get; set; }
        public DateTime? PerformanceDate { get; set; }
        public string SearchKeyword { get; set; }

        public PerformerFiltering(string name, string bandOrMusician, DateTime? performanceDate, string searchKeyword)
        {
            Name = name;
            BandOrMusician = bandOrMusician;
            PerformanceDate = performanceDate;
            SearchKeyword = searchKeyword;
        }
    }
}
