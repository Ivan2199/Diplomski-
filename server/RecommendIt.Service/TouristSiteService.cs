using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
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
    public class TouristSiteService : ITouristSiteService
    {
        private readonly ITouristSiteRepository _touristSiteRepository;
        public TouristSiteService(ITouristSiteRepository touristSiteRepository)
        {
            _touristSiteRepository = touristSiteRepository;
        }
        public async Task<PagingInfo<ITouristSitesModel>> GetAllSitesAsync(Paging paging, Sorting sort, TouristSiteFiltering filtering)
        {
            return await _touristSiteRepository.GetAllSitesAsync(paging, sort, filtering);
        }
        public async Task<ITouristSitesModel> GetTouristSiteAsync(Guid id)
        {
            return await _touristSiteRepository.GetTouristSiteAsync(id);
        }
        public async Task AddTouristSiteAsync(ITouristSitesModel site)
        {
            site.CreatedBy = GetUserId();
            site.UpdatedBy = GetUserId();
            await _touristSiteRepository.AddTouristSiteAsync(site);
        }
        public async Task UpdateTouristSiteAsync(Guid id, ITouristSitesModel site)
        {
            site.UpdatedBy = GetUserId();
            await _touristSiteRepository.UpdateTouristSiteAsync(id, site);
        }
        public async Task<ITouristSitesModel> GetPerformerByOpenTripMapIdAsync(string openTripMapId)
        {
            return await _touristSiteRepository.GetPerformerByOpenTripMapIdAsync(openTripMapId);
        }
        public async Task DeleteTouristSiteAsync(Guid id)
        {
            await _touristSiteRepository.DeleteTouristSiteAsync(id);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
