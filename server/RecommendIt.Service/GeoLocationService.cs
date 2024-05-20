using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly IGeoLoctionRepositrory _geoLocationRepository;
        public GeoLocationService(IGeoLoctionRepositrory geoLocationRepository)
        {
            _geoLocationRepository = geoLocationRepository;
        }
        public async Task<List<IGeoLocationModel>> GetAllGeoLocationsAsync()
        {
            return await _geoLocationRepository.GetAllGeoLocationsAsync();
        }
        public async Task<IGeoLocationModel> GetGeoLocationAsync(Guid id)
        {
            return await _geoLocationRepository.GetGeoLocationAsync(id);
        }
        public async Task AddGeoLocationAsync(IGeoLocationModel geoLocationModel)
        {
            geoLocationModel.CreatedBy = GetUserId();
            geoLocationModel.UpdatedBy = GetUserId();
            await _geoLocationRepository.AddGeoLocationAsync(geoLocationModel);
        }
        public async Task UpdateGeoLocationAsync(Guid id, IGeoLocationModel geoLocationData)
        {
            geoLocationData.UpdatedBy = GetUserId();
            await _geoLocationRepository.UpdateGeoLocationAsync(id, geoLocationData);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
