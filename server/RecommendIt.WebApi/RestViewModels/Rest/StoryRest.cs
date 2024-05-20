using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class StoryRest
    {
        public string Text { get; set; }
        public DateTime? DateTime { get; set; }
        public Guid LocationId { get; set; }
    }
}