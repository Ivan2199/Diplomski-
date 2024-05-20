using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class TouristSiteRest
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Guid LocationId { get; set; }
        public string Fsq_Id { get; set; }
        public double? Popularity { get; set; }
        public double? Rating { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string Email { get; set; }
        public string HoursOpen { get; set; }
        public string FacebookId { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
    }
}