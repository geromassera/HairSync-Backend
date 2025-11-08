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

        // (GetAllReviewsAsync() sigue igual que antes)
        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllWithUserAsync();
            // ... (el mapeo manual sigue igual)
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
            // --- VALIDACIÓN DE LA REGLA 1 ---
            // Verificamos si el usuario ya dejó una reseña
            bool alreadyReviewed = await _reviewRepository.HasUserReviewedAsync(userId);

            if (alreadyReviewed)
            {
                // Si ya lo hizo, lanzamos la excepción que el controlador atrapará
                throw new AlreadyReviewedException("Este usuario ya ha enviado una reseña.");
            }

            // Si llegamos acá, es porque no había reseñado.
            // El resto del código sigue igual.
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
