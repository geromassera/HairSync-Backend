using Domain.Enums;
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
        [MaxLength(25)]
        public string Name { get; set; } = string.Empty;


        [Required]
        [MaxLength(25)]
        public string Surname { get; set; } = string.Empty;


        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [MaxLength(20)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d).{7,}$",
            ErrorMessage = "La contraseña debe tener al menos 7 caracteres, una mayúscula y un número."
        )]
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
        [MaxLength(25)]
        public string? Name { get; set; }


        [MaxLength(25)]
        public string? Surname { get; set; }


        [EmailAddress]
        [MaxLength(20)]
        public string? Email { get; set; }


        [RegularExpression(
        @"^(?=.*[A-Z])(?=.*\d).{7,}$",
        ErrorMessage = "La contraseña debe tener al menos 7 caracteres, una mayúscula y un número."
    )]
        public string? Password { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }
    }


    public class AuthResultDto
    {
        public UserDto User { get; set; } = new UserDto();
        public string Token { get; set; } = string.Empty;
    }

    public class ChangeRoleRequestDto
    {
        [Required(ErrorMessage = "EL nuevo rol es requerido.")]
        public UserRole NewRole { get; set; }
    }
}
