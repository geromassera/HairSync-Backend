using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllWithUserAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        // --- IMPLEMENTACIÓN DEL NUEVO MÉTODO ---
        public async Task<bool> HasUserReviewedAsync(int userId)
        {
            // AnyAsync es súper eficiente. Deja de buscar 
            // en cuanto encuentra una coincidencia.
            return await _context.Reviews.AnyAsync(r => r.UserId == userId);
        }
    }
}
