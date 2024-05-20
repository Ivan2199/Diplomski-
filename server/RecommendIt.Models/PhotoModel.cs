using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class PhotoModel : IPhotoModel
    {
        public Guid Id { get; set; }
        public string ImagePrefix {  get; set; }
        public string ImageSuffix { get; set; }
        public Guid? TouristSiteId { get; set; }
        public Guid? StoryId { get; set; }
        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        

        public ITouristSitesModel TouristSite { get; set; }
        public IStoryModel Story { get; set; }
        public IUserModel User { get; set; }
    }
}
