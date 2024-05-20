using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models;
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
    public class PerformerService : IPerformerService
    {
        private readonly IPerformerRepository _performerRepository;
        public PerformerService(IPerformerRepository performerRepository)
        {
            _performerRepository = performerRepository;
        }
        public async Task<PagingInfo<IPerformerModel>> GetAllPerformersAsync(Paging paging, Sorting sort, PerformerFiltering filtering)
        {
            return await _performerRepository.GetAllPerformersAsync(paging, sort, filtering);
        }
        public async Task<IPerformerModel> GetPerformerAsync(Guid id)
        {
            return await _performerRepository.GetPerformerAsync(id);
        }
        public async Task AddPerformerAsync(IPerformerModel performerModel)
        {
            performerModel.CreatedBy = GetUserId();
            performerModel.UpdatedBy = GetUserId();
            await _performerRepository.AddPerformerAsync(performerModel);
        }
        public async Task UpdatePerformerAsync(Guid id, IPerformerModel performerData)
        {
            performerData.UpdatedBy = GetUserId();
            await _performerRepository.UpdatePerformerAsync(id, performerData);
        }
        public async Task<IPerformerModel> GetPerformerByJambaseIdentifierAsync(string jambaseIdentifier)
        {
            return await _performerRepository.GetPerformerByJambaseIdentifierAsync(jambaseIdentifier);
        }
        public async Task DeletePerformerAsync(Guid id)
        {
            await _performerRepository.DeletePerformerAsync(id);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
