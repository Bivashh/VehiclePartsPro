namespace VehiclePartsPro.Application.DTOs.Review;

public class CreateReviewDto
{
    public int AppointmentId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }
}

public class ReviewDto
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public int AppointmentId { get; set; }

    public string ServiceType { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}