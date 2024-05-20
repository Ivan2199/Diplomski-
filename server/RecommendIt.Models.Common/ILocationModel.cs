using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ILocationModel
    {
        Guid Id { get; set; }
        string Country { get; set; }
        string City { get; set; }
        string Village { get; set; }
        string Address { get; set; }
        string NameOfPlace { get; set; }
        string JambaseIdentifier { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        List<IGeoLocationModel> GeoLocations { get; set; }
        List<IEventModel> Events { get; set; }
        List<ITouristSitesModel> Sites { get; set; }
        List<IStoryModel> Stories { get; set; }
    }
}
