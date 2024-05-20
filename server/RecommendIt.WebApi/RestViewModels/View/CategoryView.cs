using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class CategoryView
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }

        public List<TouristSiteCategoryView> SiteCategories { get; set; }
    }
}