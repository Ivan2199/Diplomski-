using GeoTagMap.Models.Common;
using GeoTagMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common;

namespace GeoTagMap.Service.Common
{
    public interface ILocationService
    {
        Task<PagingInfo<ILocationModel>> GetAllLocationsAsync(Paging paging, Sorting sort, LocationFiltering filtering);
        Task<ILocationModel> GetLocationAsync(Guid id);
        Task AddLocationAsync(ILocationModel location);
        Task UpdateLocationAsync(Guid id, ILocationModel locationData);
        Task<ILocationModel> GetLocationByJambaseIdentifierAsync(string jambaseIdentifier);
        Task<ILocationModel> GetLocationByAddressAsync(string address);
    }
}
