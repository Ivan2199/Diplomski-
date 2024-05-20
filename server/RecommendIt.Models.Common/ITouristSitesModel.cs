using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ITouristSitesModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Link { get; set; }
        Guid? LocationId { get; set; }
        string Fsq_Id { get; set; }
        double? Popularity { get; set; }
        double? Rating { get; set; }
        string Description { get; set; }
        string WebsiteUrl { get; set; }
        string Email { get; set; }
        string HoursOpen { get; set; }
        string FacebookId { get; set; }
        string Instagram { get; set; }
        string Twitter { get; set; }


        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        ILocationModel Location { get; set; }
        List<ICommentModel> Comments { get; set; }
        List<ITouristSiteCategoryModel> SiteCategories { get; set; }
        public List<IPhotoModel> Photos { get; set; }
    }
}
