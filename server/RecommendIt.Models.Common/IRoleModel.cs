using System;
using System.Collections.Generic;

namespace GeoTagMap.Models.Common
{
    public interface IRoleModel
    {
        Guid Id { get; set; }
        string Type { get; set; }

        List<IUserModel> Users { get; set; }
    }
}