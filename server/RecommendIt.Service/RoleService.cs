using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<IRoleModel>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }

        public async Task<IRoleModel> GetRoleByIdAsync(Guid id)
        {
            return await _roleRepository.GetRoleByIdAsync(id);
        }

        public async Task UpdateRoleAsync(Guid id, IRoleModel role)
        {
            await _roleRepository.UpdateRoleAsync(id, role);
        }
    }
}