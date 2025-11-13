using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly ApplicationDbContext _dbcontext;

        public UserRepository(ApplicationDbContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.ToLowerInvariant();
            return await _dbcontext.Users
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _dbcontext.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbcontext.Users.ToListAsync();
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            return await _dbcontext.Users
                .FirstOrDefaultAsync(u => u.Phone == phone);
        }
        
        public async Task<List<User>> GetBarbersByBranchAsync(int branchId)
        {
            return await _dbcontext.Users
                .AsNoTracking()
                .Where(u => u.BranchId == branchId && u.Role == UserRole.Barber)
                .OrderBy(u => u.Name).ThenBy(u => u.Surname)
                .ToListAsync();
        }
    }
}
