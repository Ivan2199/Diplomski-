using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class TouristSiteCategoryModel : ITouristSiteCategoryModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TouristSiteId { get; set;}

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }
    }
}
