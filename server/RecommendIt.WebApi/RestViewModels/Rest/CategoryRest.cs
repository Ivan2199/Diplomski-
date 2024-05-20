using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class CategoryRest
    {
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Fsq_CategoryId { get; set; }
    }
}