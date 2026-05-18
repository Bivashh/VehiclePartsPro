using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs.Review;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;

    public ReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewDto> CreateReviewAsync(
        string userId,
        CreateReviewDto dto)
    {
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (customer == null)
            throw new Exception("Customer not found.");

        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a =>
                a.Id == dto.AppointmentId &&
                a.CustomerId == customer.Id);

        if (appointment == null)
            throw new Exception("Appointment not found.");

        if (appointment.Status != "Completed")
            throw new Exception(
                "Review can only be added after appointment completion.");

        var existingReview = await _db.Reviews
            .FirstOrDefaultAsync(r =>
                r.AppointmentId == dto.AppointmentId);

        if (existingReview != null)
            throw new Exception(
                "Review already exists for this appointment.");

        var review = new Review
        {
            CustomerId = customer.Id,
            AppointmentId = appointment.Id,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);

        await _db.SaveChangesAsync();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == customer.UserId);

        return new ReviewDto
        {
            Id = review.Id,
            CustomerId = customer.Id,
            CustomerName = user?.FullName ?? "",
            AppointmentId = appointment.Id,
            ServiceType = appointment.ServiceType,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task<List<ReviewDto>> GetMyReviewsAsync(string userId)
    {
        return await _db.Reviews
            .Include(r => r.Appointment)
            .Include(r => r.Customer)
            .Where(r => r.Customer.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                AppointmentId = r.AppointmentId,
                ServiceType = r.Appointment.ServiceType,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<ReviewDto>> GetAllReviewsAsync()
    {
        return await _db.Reviews
            .Include(r => r.Appointment)
            .Include(r => r.Customer)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                AppointmentId = r.AppointmentId,
                ServiceType = r.Appointment.ServiceType,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }
}