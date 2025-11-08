using Application.External;
using Application.Models;
using Domain.Entities;
using Domain.Interfaces;
using Application.Exceptions;

namespace Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllWithUserAsync();
            return reviews.Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Text = r.Text,
                CreatedAt = r.CreatedAt,
                ClientName = r.User != null ? $"{r.User.Name} {r.User.Surname}" : "Usuario Anónimo"
            });
        }

        public async Task CreateReviewAsync(CreateReviewDto dto, int userId)
        {
            bool alreadyReviewed = await _reviewRepository.HasUserReviewedAsync(userId);

            if (alreadyReviewed)
            {
                throw new AlreadyReviewedException("Este usuario ya ha enviado una reseña.");
            }

            var review = new Review
            {
                Rating = dto.Rating,
                Text = dto.Text,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }
        private bool isAdmin(int requesterId, int customerId)
        {
            return requesterId == customerId; 
        }
    }
}
