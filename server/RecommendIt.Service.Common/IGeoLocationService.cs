using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface IGeoLocationService
    {
        Task<List<IGeoLocationModel>> GetAllGeoLocationsAsync();
        Task<IGeoLocationModel> GetGeoLocationAsync(Guid id);
        Task AddGeoLocationAsync(IGeoLocationModel geoLocationModel);
        Task UpdateGeoLocationAsync(Guid id, IGeoLocationModel geoLocationData);
    }
}
