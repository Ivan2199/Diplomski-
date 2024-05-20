using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class TouristSiteCategoryView
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TouristSiteId { get; set; }
    }
}