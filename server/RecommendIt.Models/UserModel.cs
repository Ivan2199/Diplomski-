using GeoTagMap.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeoTagMap.Models
{
    public class UserModel : IUserModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Image { get; set; }

        public IRoleModel UserRole { get; set; }
    }
}