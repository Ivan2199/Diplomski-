using System;

namespace GeoTagMap.RestViewModels
{
    public class UserModelView
    {
        public Guid Id { get; set; }
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public RoleModelView UserRole { get; set; }
    }
}