using VehiclePartsPro.Application.DTOs;

namespace VehiclePartsPro.Application.Interfaces;

public interface IOverduePaymentReminderService
{
    Task<List<OverduePaymentReminderDto>> SendOverduePaymentRemindersAsync();
}