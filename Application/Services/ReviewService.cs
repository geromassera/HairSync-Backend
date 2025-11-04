using Application.External;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        // private readonly IAppointmentRepository _appointmentRepository; // falta: descomentar

        public ReviewService(IReviewRepository reviewRepository /*, IAppointmentRepository appointmentRepository */)
        {
            _reviewRepository = reviewRepository;
            // _appointmentRepository = appointmentRepository; falta: descomentar
        }

        public async Task<ReviewDto?> GetByAppointmentAsync(int appointmentId, int requesterUserId, bool isAdmin)
        {
            // falta: agregar validacion de turno cuando exista IAppointmentRepository
            var review = await _reviewRepository.GetByAppointmentIdAsync(appointmentId);
            if (review == null)
                return null;

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                AppointmentId = review.AppointmentId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        public async Task<ReviewDto> CreateAsync(int appointmentId, int requesterUserId, CreateReviewDto dto)
        {
            // falta: validar turno y dueño del turno cuando exista IAppointmentRepository
            var existingReview = await _reviewRepository.GetByAppointmentIdAsync(appointmentId);
            if (existingReview != null)
                throw new InvalidOperationException("Ya existe una reseña para este turno.");

            var review = new Review
            {
                AppointmentId = appointmentId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                AppointmentId = review.AppointmentId,
                Rating = review.Rating,
                Comment = review.Comment
            };
        }

        public async Task UpdateAsync(int reviewId, int requesterUserId, UpdateReviewDto dto, bool isAdmin)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException("La reseña no existe.");

            // falta: validar que el usuario sea dueño del turno o admin

            if (dto.Rating.HasValue)
                review.Rating = dto.Rating.Value;
            if (!string.IsNullOrWhiteSpace(dto.Comment))
                review.Comment = dto.Comment;

            _reviewRepository.Update(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId, int requesterUserId, bool isAdmin)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId)
                ?? throw new KeyNotFoundException("La reseña no existe.");

            // falta: validar que el usuario sea dueño del turno o admin

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveChangesAsync();
        }
    }
}
