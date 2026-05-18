using Microsoft.EntityFrameworkCore;
using VehiclePartsPro.Application.DTOs;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Infrastructure.Data;

namespace VehiclePartsPro.Infrastructure.Services;

public class OverduePaymentReminderService : IOverduePaymentReminderService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _emailService;

    public OverduePaymentReminderService(
        AppDbContext db,
        IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<List<OverduePaymentReminderDto>> SendOverduePaymentRemindersAsync()
    {
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

        var overdueCustomers = await _db.Customers
            .Where(c =>
                c.CreditBalance > 0 &&
                c.CreditDueDate != null &&
                c.CreditDueDate <= oneMonthAgo)
            .ToListAsync();

        var results = new List<OverduePaymentReminderDto>();

        foreach (var customer in overdueCustomers)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == customer.UserId);

            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                results.Add(new OverduePaymentReminderDto
                {
                    CustomerId = customer.Id,
                    CustomerName = user?.FullName ?? "Unknown",
                    CustomerEmail = user?.Email ?? "No email",
                    CreditBalance = customer.CreditBalance,
                    CreditDueDate = customer.CreditDueDate,
                    Status = "Skipped - customer email not found"
                });

                continue;
            }

            var subject = "Overdue Payment Reminder - Vehicle Parts Pro";

            var body = $@"
                <h2>Vehicle Parts Pro - Payment Reminder</h2>

                <p>Hello {user.FullName},</p>

                <p>This is a reminder that your credit payment is overdue.</p>

                <p><strong>Outstanding Balance:</strong> Rs. {customer.CreditBalance}</p>
                <p><strong>Due Date:</strong> {customer.CreditDueDate:yyyy-MM-dd}</p>

                <p>Please clear your pending payment as soon as possible.</p>

                <p>Regards,<br/>Vehicle Parts Pro</p>
            ";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            results.Add(new OverduePaymentReminderDto
            {
                CustomerId = customer.Id,
                CustomerName = user.FullName,
                CustomerEmail = user.Email,
                CreditBalance = customer.CreditBalance,
                CreditDueDate = customer.CreditDueDate,
                Status = "Reminder email sent"
            });
        }

        return results;
    }
}