using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface IGeoLocationModel
    {
        Guid Id { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        Guid LocationId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        ILocationModel Location { get; set; }
    }
}
