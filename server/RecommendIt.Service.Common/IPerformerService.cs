﻿using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface IPerformerService
    {
        Task<PagingInfo<IPerformerModel>> GetAllPerformersAsync(Paging pagign, Sorting sort, PerformerFiltering filtering);
        Task<IPerformerModel> GetPerformerAsync(Guid id);
        Task AddPerformerAsync(IPerformerModel performerModel);
        Task UpdatePerformerAsync(Guid id, IPerformerModel performerData);
        Task<IPerformerModel> GetPerformerByJambaseIdentifierAsync(string jambaseIdentifier);
        Task DeletePerformerAsync(Guid id);
    }
}
