using GeoTagMap.Models.Common;
using GeoTagMap.RestViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class CommentView
    {
        public Guid Id { get; set; }
        public string Text { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }

        public EventView Event { get; set; }
        public StoryView Story { get; set; }
        public TouristSiteView Site { get; set; }
        public UserModelView User { get; set; }
    }
}