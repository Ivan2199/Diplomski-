using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface ILocationRepository
    {
        Task<PagingInfo<ILocationModel>> GetAllLocationsAsync(Paging paging, Sorting sort, LocationFiltering filtering);
        Task<ILocationModel> GetLocationAsync(Guid id);
        Task AddLocationAsync(ILocationModel location);
        Task UpdateLocationAsync(Guid id, ILocationModel locationData);
        Task<ILocationModel> GetLocationByJambaseIdentifierAsync(string jambaseIdentifier);
        Task<ILocationModel> GetLocationByAddressAsync(string address);
    }
}
