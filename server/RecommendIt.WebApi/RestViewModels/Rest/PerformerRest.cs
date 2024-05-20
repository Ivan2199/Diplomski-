using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class PerformerRest
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string BandOrMusician { get; set; }
        public int? NumOfUpcomingEvents { get; set; }
        public DateTime? PerformanceDate { get; set; }
        public bool? DateIsConfirmed { get; set; }
        public string JambaseIdentifier { get; set; }
    }
}