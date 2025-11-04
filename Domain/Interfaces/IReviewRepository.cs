using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id);
        Task<Review?> GetByAppointmentIdAsync(int appointmentId);
        Task AddAsync(Review review);
        void Update(Review review);
        void Remove(Review review);
        Task SaveChangesAsync();
    }
}
