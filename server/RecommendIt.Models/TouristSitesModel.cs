using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class TouristSitesModel : ITouristSitesModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public Guid? LocationId { get; set; }
        public string Fsq_Id { get; set; }
        public double? Popularity { get; set; }
        public double? Rating { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string Email { get; set; }
        public string HoursOpen { get; set; }
        public string FacebookId { get; set; }
        public string Instagram {  get; set; }
        public string Twitter { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public ILocationModel Location { get; set; }
        public List<ICommentModel> Comments { get; set; }
        public List<ITouristSiteCategoryModel> SiteCategories { get; set; }
        public List<IPhotoModel> Photos { get; set; }
    }
}
