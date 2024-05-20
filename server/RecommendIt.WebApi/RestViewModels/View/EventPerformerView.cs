using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class EventPerformerView
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid PerformerId { get; set; }
    }
}