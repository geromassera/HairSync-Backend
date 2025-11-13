using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
namespace Domain.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);

        Task<IEnumerable<User>> GetAllAsync();

        Task<User?> GetByPhoneAsync(string phone);

        Task<List<User>> GetBarbersByBranchAsync(int branchId);
    }
}
