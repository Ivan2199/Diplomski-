using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class EventRest
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string EventStatus { get; set; }
        public string Image { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAccessibleForFree { get; set; }
        public string Type { get; set; }

        public Guid? TicketInformationId { get; set; }
        public Guid? LocationId { get; set; }
    }
}