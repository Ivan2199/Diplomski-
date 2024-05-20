using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ICategoryModel
    {
        Guid Id { get; set; }
        string Type { get; set; }
        string Icon { get; set; }
        string Fsq_CategoryId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        List<ITouristSiteCategoryModel> SiteCategories { get; set; }
    }
}
