using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface IRoleRepository
    {
        Task<List<IRoleModel>> GetAllRolesAsync();

        Task UpdateRoleAsync(Guid id, IRoleModel role);
        Task<Guid> FindRoleByTypeAsync(string type);

        Task<IRoleModel> GetRoleByIdAsync(Guid id);
    }
}