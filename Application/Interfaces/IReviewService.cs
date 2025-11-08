using Application.Models;

namespace Application.External
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();

        // Pasamos el DTO y el ID del usuario que la está creando
        Task CreateReviewAsync(CreateReviewDto dto, int userId);
    }
}
