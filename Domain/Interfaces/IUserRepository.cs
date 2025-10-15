using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        /// <summary>
        /// Finds a user by email.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Gets all users by role.
        /// </summary>
        Task<IEnumerable<User>> GetAlleAsync();
    }
}
