using System;
using System.ComponentModel.DataAnnotations;

namespace GeoTagMap.Models.Common
{
    public interface IUserModel
    {
        Guid Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        string Password { get; set; }
        Guid RoleId { get; set; }
        bool IsActive { get; set; }
        string Image { get; set; }

        IRoleModel UserRole { get; set; }
    }
}