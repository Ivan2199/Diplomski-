using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class LocationView
    {
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Village { get; set; }
        public string Address { get; set; }
        public string NameOfPlace { get; set; }

        public List<GeoLocationView> GeoLocations { get; set; }
        public List<EventView> Events { get; set; }
        public List<TouristSiteView> Sites { get; set; }
        public List<StoryView> Stories { get; set; }
    }
}