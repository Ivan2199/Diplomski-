using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace GeoTagMap.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<PagingInfo<IUserModel>> GetAllUsersAsync(Paging paging, Sorting sort, UserFiltering filtering)
        {
            return await _userRepository.GetAllUsersAsync(paging, sort, filtering);
        }

        public async Task<IUserModel> GetLoggedUserByIdAsync()
        {
            var id = GetUserId();
            return await _userRepository.GetUserByIdAsync(id);
        }
        public async Task<IUserModel> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task AddUserAsync(IUserModel user)
        {
            await _userRepository.AddUserAsync(user);
        }

        public async Task RegisterUserAsync(IUserModel user)
        {
            Guid roleId = await _roleRepository.FindRoleByTypeAsync("User");
            user.RoleId = roleId;
            var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
            Validator.ValidateObject(user, validationContext, validateAllProperties: true);

            if (await IsUsernameTakenAsync(user.Username))
            {
                throw new ArgumentException("Username is already taken.");
            }

            if (await IsEmailTakenAsync(user.Email))
            {
                throw new ArgumentException("Email is already taken.");
            }

            user.Password = HashPassword(user.Password);

            await _userRepository.AddUserAsync(user);
        }

        private async Task<bool> IsUsernameTakenAsync(string username)
        {
            var existingUsers = await _userRepository.GetAllUsersAsync();
            foreach (var user in existingUsers)
            {
                if (user.Username == username)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<bool> IsEmailTakenAsync(string email)
        {
            var existingUsers = await _userRepository.GetAllUsersAsync();
            foreach (var user in existingUsers)
            {
                if (user.Email == email)
                {
                    return true;
                }
            }

            return false;
        }

        private string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }

        public async Task UpdateUserAsync(Guid id, IUserModel updatedUser, bool isChangeToAdmin, bool isUnbanned, string oldPassword = "")
        {
            try
            {
                if (isChangeToAdmin)
                {
                    Guid roleId = await _roleRepository.FindRoleByTypeAsync("Admin");
                    updatedUser.RoleId = roleId;
                }
                else if (isUnbanned)
                {
                    Guid roleId = await _roleRepository.FindRoleByTypeAsync("User");
                    updatedUser.RoleId = roleId;
                }
                else
                {
                    IUserModel currentUser = await GetLoggedUserByIdAsync();

                    if (!string.IsNullOrEmpty(oldPassword))
                    {
                        if (BCrypt.Net.BCrypt.Verify(oldPassword, currentUser.Password))
                        {
                            updatedUser.Password = HashPassword(updatedUser.Password);
                        }
                        else
                        {
                            throw new Exception("Old password is not correct.");
                        }
                    }
                }
                await _userRepository.UpdateUserAsync(id, updatedUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task DeleteUserAsync(Guid id)
        {
            Guid roleId = await _roleRepository.FindRoleByTypeAsync("BannedUser");
            await _userRepository.DeleteUserAsync(id, roleId); 
        }

        public async Task<IUserModel> ValidateUserAsync(UserFilter user)
        {
            var currentUser = await _userRepository.FindUserAsync(user);

            if (currentUser != null && BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password))
            {
                return currentUser;
            }

            return null;
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("UserId")?.Value);
        }
    }
}