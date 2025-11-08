using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllWithUserAsync();

        Task AddAsync(Review review);
        Task<bool> HasUserReviewedAsync(int userId);
    }
}
