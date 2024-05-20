using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class TouristSiteCategoryRest
    {
        public Guid CategoryId { get; set; }
        public Guid TouristSiteId { get; set; }
    }
}