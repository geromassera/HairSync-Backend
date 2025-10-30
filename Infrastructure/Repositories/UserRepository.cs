using Domain.Interfaces;
using Domain.Entities;
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

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbcontext.Users.ToListAsync();
        }
    }
}
