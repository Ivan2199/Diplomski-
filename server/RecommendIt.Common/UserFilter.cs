using System;

namespace GeoTagMap.Common
{
    public class UserFilter
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RoleType { get; set; }

        public UserFilter(string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
        }

        public UserFilter(Guid userId, string userName, string email, string role)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.Email = email;
            this.RoleType = role;
        }
    }
}