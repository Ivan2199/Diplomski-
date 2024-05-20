using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface ITouristSiteRepository
    {
        Task<PagingInfo<ITouristSitesModel>> GetAllSitesAsync(Paging paging, Sorting sort, TouristSiteFiltering filtering);
        Task<ITouristSitesModel> GetTouristSiteAsync(Guid id);
        Task AddTouristSiteAsync(ITouristSitesModel site);
        Task UpdateTouristSiteAsync(Guid id, ITouristSitesModel site);
        Task<ITouristSitesModel> GetPerformerByOpenTripMapIdAsync(string openTripMapId);
        Task DeleteTouristSiteAsync(Guid id);
    }
}
