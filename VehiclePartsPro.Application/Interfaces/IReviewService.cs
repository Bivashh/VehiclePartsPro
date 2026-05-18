using VehiclePartsPro.Application.DTOs.Review;

namespace VehiclePartsPro.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(
        string userId,
        CreateReviewDto dto);

    Task<List<ReviewDto>> GetMyReviewsAsync(string userId);

    Task<List<ReviewDto>> GetAllReviewsAsync();
}