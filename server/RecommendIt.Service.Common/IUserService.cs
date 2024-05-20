using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace GeoTagMap.Service.Common
{
    public interface IUserService
    {
        Task<PagingInfo<IUserModel>> GetAllUsersAsync(Paging paging, Sorting sort, UserFiltering filtering);

        Task<IUserModel> GetLoggedUserByIdAsync();
        Task<IUserModel> GetUserByIdAsync(Guid id);

        Task AddUserAsync(IUserModel user);

        Task UpdateUserAsync(Guid id, IUserModel updatedUser, bool isChangeToAdmin, bool isUnbanned, string oldPassword = "");

        Task DeleteUserAsync(Guid id);

        Task RegisterUserAsync(IUserModel user);

        Task<IUserModel> ValidateUserAsync(UserFilter user);
    }
}