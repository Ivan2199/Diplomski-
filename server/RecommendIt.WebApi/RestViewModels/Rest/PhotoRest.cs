using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class PhotoRest
    {
        public string ImagePrefix { get; set; }
        public string ImageSuffix { get; set; }
        public Guid? TouristSiteId { get; set; }
        public Guid? StoryId { get; set; }
    }
}