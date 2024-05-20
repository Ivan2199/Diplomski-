using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class PerformerView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string BandOrMusician { get; set; }
        public int? NumOfUpcomingEvents { get; set; }
        public DateTime? PerformanceDate { get; set; }
        public bool? DateIsConfirmed { get; set; }

        public List<EventPerformerView> EventPerformers { get; set; }
    }
}