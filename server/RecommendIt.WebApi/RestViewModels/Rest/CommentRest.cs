using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class CommentRest
    {
        public string Text { get; set; }
        public Guid? EventId { get; set; }
        public Guid? TouristSiteId { get; set; }
        public Guid? StoryId { get; set; }
    }
}