using System;

namespace GeoTagMap.RestViewModels
{
    public class UserModelRest
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Image {  get; set; }
        public bool IsActive { get; set; }

    }
}