using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class CategoryModel : ICategoryModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Fsq_CategoryId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public List<ITouristSiteCategoryModel> SiteCategories { get; set; }

    }
}
