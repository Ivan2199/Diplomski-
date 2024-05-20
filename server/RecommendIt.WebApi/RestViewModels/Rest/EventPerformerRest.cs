using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class EventPerformerRest
    {
        public Guid EventId { get; set; }
        public Guid PerformerId { get; set; }
    }
}