using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class LocationModel : ILocationModel
    {
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string City {  get; set; }
        public string Village { get; set; }
        public string Address { get; set; }
        public string NameOfPlace { get; set; }
        public string JambaseIdentifier { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public List<IGeoLocationModel> GeoLocations { get; set; }
        public List<IEventModel> Events { get; set; }
        public List<ITouristSitesModel> Sites { get; set; }
        public List<IStoryModel> Stories { get; set; }
    }
}
