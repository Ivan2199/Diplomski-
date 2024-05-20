using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface IUserRepository
    {
        Task<PagingInfo<IUserModel>> GetAllUsersAsync(Paging paging, Sorting sort, UserFiltering filtering);
        Task<List<IUserModel>> GetAllUsersAsync();

        Task<IUserModel> GetUserByIdAsync(Guid id);

        Task<IUserModel> FindUserAsync(UserFilter userFilter);

        Task AddUserAsync(IUserModel user);

        Task UpdateUserAsync(Guid id, IUserModel user);

        Task DeleteUserAsync(Guid id, Guid bannedRoleId);
    }
}