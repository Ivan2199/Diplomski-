using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface IPhotoModel
    {
        Guid Id { get; set; }
        string ImagePrefix { get; set; }
        string ImageSuffix { get; set; }
        Guid? TouristSiteId { get; set; }
        Guid? StoryId { get; set; }
        Guid UserId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool IsActive { get; set; }

        ITouristSitesModel TouristSite { get; set; }
        IStoryModel Story { get; set; }
        IUserModel User { get; set; }
    }
}
