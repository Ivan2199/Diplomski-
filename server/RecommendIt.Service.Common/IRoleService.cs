using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface IRoleService
    {
        Task<List<IRoleModel>> GetAllRolesAsync();

        Task<IRoleModel> GetRoleByIdAsync(Guid id);

        Task UpdateRoleAsync(Guid id, IRoleModel role);
    }
}