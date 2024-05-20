using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ITouristSiteCategoryModel
    {
        Guid Id { get; set; }
        Guid CategoryId { get; set; }
        Guid TouristSiteId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }
    }
}
