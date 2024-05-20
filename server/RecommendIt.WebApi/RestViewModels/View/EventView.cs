using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class EventView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string EventStatus { get; set; }
        public string Image { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAccessibleForFree { get; set; }
        public string Type { get; set; }


        public TicketInformationView TicketInformation { get; set; }
        public List<EventPerformerView> EventPerformers { get; set; }
        public LocationView Location { get; set; }
        public List<CommentView> Comments { get; set; }
    }
}