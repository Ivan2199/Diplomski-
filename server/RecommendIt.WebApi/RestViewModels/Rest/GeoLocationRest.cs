using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class GeoLocationRest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid LocationId { get; set; }
    }
}