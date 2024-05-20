using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.RestViewModels;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeoTagMap.WebApi.Controllers
{
    [Authorize]
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                List<RoleModelView> rolesView = new List<RoleModelView>();
                foreach (var role in roles)
                {
                    rolesView.Add(MapRoleView(role));
                }
                if (rolesView.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }

                return Request.CreateResponse(HttpStatusCode.OK, rolesView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);

                if (role == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }

                return Request.CreateResponse(HttpStatusCode.OK, role);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] RoleModelRest role)
        {
            try
            {
                var roleCurrent = MapRoleRest(role);
                if (roleCurrent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                await _roleService.UpdateRoleAsync(id, roleCurrent);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IRoleModel MapRoleRest(RoleModelRest role)
        {
            return new RoleModel
            {
                Id = role.Id,
                Type = role.Type,
            };
        }

        private RoleModelView MapRoleView(IRoleModel role)
        {
            return new RoleModelView
            {
                Id = role.Id,
                Type = role.Type,
                Users = role.Users.Select(MapUser).ToList()
            };
        }

        private UserModelView MapUser(IUserModel user)
        {
            return new UserModelView
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username
            };
        }
    }
}