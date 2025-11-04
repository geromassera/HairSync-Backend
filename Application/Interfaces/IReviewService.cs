using Application.Models;

namespace Application.External
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetByAppointmentAsync(int appointmentId, int requesterUserId, bool isAdmin);
        Task<ReviewDto> CreateAsync(int appointmentId, int requesterUserId, CreateReviewDto dto);
        Task UpdateAsync(int reviewId, int requesterUserId, UpdateReviewDto dto, bool isAdmin);
        Task DeleteAsync(int reviewId, int requesterUserId, bool isAdmin);
    }
}
