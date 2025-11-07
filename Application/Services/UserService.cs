using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Application.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.ListAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user is null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(RegisterDto dto)
        {

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = hashedPassword,
                Role = UserRole.Client
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                throw new NotFoundException($"Usuario con Id {id} no encontrado.");
            }
            
            if (dto.Email is not null && dto.Email != user.Email)
            {
                var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingEmail is not null && existingEmail.UserId != id)
                {
                    throw new ForbiddenAccessException($"El email ya esta en uso por otra cuenta.");
                }
                user.Email = dto.Email;
            }
            if (dto.Name is not null)
                user.Name = dto.Name;
            if (dto.Surname is not null)
                user.Surname = dto.Surname;
            if (dto.Phone is not null)
                user.Phone = dto.Phone;
            if (dto.Password is not null)
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepository.SaveChangesAsync();

            return MapToDto(user);


        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return false;

            await _userRepository.DeleteAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task ChangeUserRoleAsync(int userId, UserRole newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new NotFoundException($"No se encontro un usuario con el Id {userId}.");
            }

            if (user.Role == UserRole.Admin)
            {
                throw new ForbiddenAccessException("No esta permitido modificar el rol de un Administrador.");
            }

            if(user.Role == newRole)
            {
                return;
            }

            user.Role = newRole;

            await _userRepository.SaveChangesAsync();
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString()
            };
        }

        public async Task AssignBranchToBarberAsync(int userId, int branchId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                throw new NotFoundException($"No se encontró un usuario con el Id {userId}.");

            if (user.Role != UserRole.Barber)
                throw new ForbiddenAccessException("Solo los usuarios con rol Barber pueden tener una sucursal asignada.");
          
            if (branchId != 1 && branchId != 2)
                throw new ForbiddenAccessException("La sucursal indicada no existe. Solo se permiten las sucursales 1 o 2.");

            user.BranchId = branchId;
            await _userRepository.SaveChangesAsync();
        }

    }
}