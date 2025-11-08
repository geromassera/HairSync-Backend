using Application.Models;

namespace Application.External
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();

        Task CreateReviewAsync(CreateReviewDto dto, int userId);
    }
}
