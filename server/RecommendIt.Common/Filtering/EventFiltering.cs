using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Common.Filtering
{
    public class EventFiltering
    {
        public string Name { get; set; }
        public string EventStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public string Type { get; set; }
        public bool? IsAccessibleForFree {  get; set; }
        public string SearchKeyword { get; set; }

        public EventFiltering(string name, string eventStatus, DateTime? startDate, string type, bool? isAccessibleForFree, string searchKeyword)
        {
            Name = name;
            EventStatus = eventStatus;
            StartDate = startDate;
            Type = type;
            IsAccessibleForFree = isAccessibleForFree;
            SearchKeyword = searchKeyword;
        }
    }
}
