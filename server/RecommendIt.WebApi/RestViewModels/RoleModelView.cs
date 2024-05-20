using System;
using System.Collections.Generic;

namespace GeoTagMap.RestViewModels
{
    public class RoleModelView
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public List<UserModelView> Users { get; set; }
    }
}