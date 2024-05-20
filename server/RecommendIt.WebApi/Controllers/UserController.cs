using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.RestViewModels;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace GeoTagMap.WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Username",
            string sortOrder = "ASC",
            string username = "",
            string firstName = "",
            string lastName = "")
        {
            try
            {
                Paging paging = new Paging(pageNumber, pageSize);
                Sorting sort = new Sorting(orderBy, sortOrder);
                UserFiltering filtering = new UserFiltering(username, firstName, lastName);

                var pagingInfo = await _userService.GetAllUsersAsync(paging, sort, filtering);

                if (pagingInfo.List.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }

                return Request.CreateResponse(HttpStatusCode.OK, pagingInfo);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("getloggeduser")]
        public async Task<HttpResponseMessage> GetLoggedUserAsync()
        {
            try
            {
                var user = await _userService.GetLoggedUserByIdAsync();
                UserModelView userView = MapUserView(user);
                if (userView == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }

                return Request.CreateResponse(HttpStatusCode.OK, userView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                UserModelView userView = MapUserView(user);
                if (userView is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }

                return Request.CreateResponse(HttpStatusCode.OK, userView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("userinfo")]
        [HttpGet]
        public IHttpActionResult GetUserInfo()
        {
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Unable to retrieve user information.");
            }

            var userId = identity.FindFirst("UserId")?.Value;
            var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = identity.FindFirst(ClaimTypes.Role)?.Value;

            Console.WriteLine($"UserId: {userId}, UserName: {userName}, UserEmail: {userEmail}, UserRole: {userRole}");

            var userInfo = new
            {
                UserId = userId,
                UserName = userName,
                UserEmail = userEmail,
                UserRole = userRole
            };

            return Ok(userInfo);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync([FromBody] UserModelRest userRest)
        {
            try
            {
                if (userRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                IUserModel user = MapUser(userRest);
                await _userService.AddUserAsync(user);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<HttpResponseMessage> RegisterAsync([FromBody] UserModelRest userRest)
        {
            try
            {
                IUserModel user = MapUser(userRest);
                await _userService.RegisterUserAsync(user);

                return Request.CreateResponse(HttpStatusCode.Created, "User registered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "User, Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] UserModelRest userRest, bool isChangeToAdmin = false, bool isUnbanned = false)
        {
            try
            {
                IUserModel user = MapUser(userRest);

                if (user is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No user with that id was found");
                }

                await _userService.UpdateUserAsync(id, user, isChangeToAdmin, isUnbanned, userRest.OldPassword);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                var user = await _userService.GetLoggedUserByIdAsync();
                if (user is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No user with that id was found");
                }

                await _userService.DeleteUserAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "User has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IUserModel MapUser(UserModelRest userRest)
        {
            return new UserModel
            {
                Id = userRest.Id,
                Username = userRest.Username,
                FirstName = userRest.FirstName,
                LastName = userRest.LastName,
                Email = userRest.Email,
                Password = userRest.NewPassword,
                Image = userRest.Image ?? null,
                IsActive = true,
            };
        }

        private UserModelView MapUserView(IUserModel user)
        {
            return new UserModelView
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Image = user.Image,
                IsActive = user.IsActive,
                UserRole = MapRoleView(user.UserRole)
            };
        }

        private RoleModelView MapRoleView(IRoleModel role)
        {
            return new RoleModelView
            {
                Type = role.Type,
            };
        }
    }
}
