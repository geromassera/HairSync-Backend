using Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<UserDto> CreateUserAsync(RegisterDto registerDto);

        Task<bool> UpdateUserAsync(int id, UpdateUserDto updateUserDto);

        Task<bool> DeleteUserAsync(int id);
    }
}