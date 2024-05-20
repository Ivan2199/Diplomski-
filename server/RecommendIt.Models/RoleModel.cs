using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;

namespace GeoTagMap.Models
{
    public class RoleModel : IRoleModel
    {
        public Guid Id { get; set; }
        public string Type { get; set; }

        public List<IUserModel> Users { get; set; }
    }
}