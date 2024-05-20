using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class GeoLocationView
    {
        public Guid Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public LocationView Location { get; set; }
    }
}