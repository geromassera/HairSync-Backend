using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;


        [Required]
        public string Surname { get; set; } = string.Empty;


        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MinLength(7)]
        public string Password { get; set; } = string.Empty;
    }


    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;


        [Required]
        public string Password { get; set; } = string.Empty;
    }


    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }


        [MaxLength(100)]
        public string? Surname { get; set; }


        [EmailAddress]
        public string? Email { get; set; }


        [MinLength(6)]
        public string? Password { get; set; }
    }


    public class AuthResultDto
    {
        public UserDto User { get; set; } = new UserDto();
        public string Token { get; set; } = string.Empty;
    }
}
