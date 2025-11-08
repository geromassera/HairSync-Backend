using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IReviewRepository
    {
        // Trae todas las reviews, incluyendo los datos del User asociado
        Task<IEnumerable<Review>> GetAllWithUserAsync();

        // Agrega una nueva review
        Task AddAsync(Review review);
        Task<bool> HasUserReviewedAsync(int userId);
    }
}
