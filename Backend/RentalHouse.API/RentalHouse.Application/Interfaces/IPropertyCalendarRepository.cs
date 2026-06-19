using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IPropertyCalendarRepository : IRepository<PropertyCalendar>
{
    Task<bool> IsDateRangeAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut);
    Task MarkDatesAsBookedAsync(int propertyId, DateTime checkIn, DateTime checkOut);
    Task ReleaseDatesAsync(int propertyId, DateTime checkIn, DateTime checkOut);
}