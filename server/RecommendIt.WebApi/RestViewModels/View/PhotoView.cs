using GeoTagMap.Models.Common;
using GeoTagMap.RestViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class PhotoView
    {
        public Guid Id { get; set; }
        public string ImagePrefix { get; set; }
        public string ImageSuffix { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }


        public TouristSiteView TouristSite { get; set; }
        public StoryView Story { get; set; }
        public UserModelView User { get; set; }
    }
}