using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Domain.Enums;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class PropertyCalendarRepository : GenericRepository<PropertyCalendar>, IPropertyCalendarRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyCalendarRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsDateRangeAvailableAsync(int propertyId, DateTime checkIn, DateTime checkOut)
    {
        var overlappingCount = await _context.Set<PropertyCalendar>()
            .CountAsync(pc => pc.PropertyId == propertyId
                           && pc.Date >= checkIn.Date
                           && pc.Date < checkOut.Date
                           && (pc.Status == CalendarStatus.Booked || pc.Status == CalendarStatus.Blocked));

        return overlappingCount == 0;
    }

    public async Task MarkDatesAsBookedAsync(int propertyId, DateTime checkIn, DateTime checkOut)
    {
        var existingCalendars = await _context.Set<PropertyCalendar>()
            .Where(pc => pc.PropertyId == propertyId
                      && pc.Date >= checkIn.Date
                      && pc.Date < checkOut.Date)
            .ToDictionaryAsync(pc => pc.Date.Date);

        for (var date = checkIn.Date; date < checkOut.Date; date = date.AddDays(1))
        {
            if (existingCalendars.TryGetValue(date, out var calendar))
            {
                calendar.Status = CalendarStatus.Booked;
                _context.Set<PropertyCalendar>().Update(calendar);
            }
            else
            {
                var newCalendar = new PropertyCalendar
                {
                    PropertyId = propertyId,
                    Date = date,
                    Status = CalendarStatus.Booked
                };
                await _context.Set<PropertyCalendar>().AddAsync(newCalendar);
            }
        }
    }

    public async Task ReleaseDatesAsync(int propertyId, DateTime checkIn, DateTime checkOut)
    {
        var datesToRelease = await _dbSet
            .Where(c => c.PropertyId == propertyId &&
                        c.Date >= checkIn.Date &&
                        c.Date < checkOut.Date &&
                        c.Status == CalendarStatus.Booked)
            .ToListAsync();

        foreach (var date in datesToRelease)
        {
            date.Status = CalendarStatus.Available; // Nhả lịch lại thành trống
        }
    }

    public async Task<List<string>> GetBookedDatesAsync(int propertyId)
    {
        var today = DateTime.UtcNow.Date; // Lấy ngày hôm nay
        return await _dbSet
            .Where(c => c.PropertyId == propertyId &&
                        (c.Status == CalendarStatus.Booked || c.Status == CalendarStatus.Blocked) &&
                        c.Date >= today)
            .Select(c => c.Date.ToString("yyyy-MM-dd"))
            .ToListAsync();
    }
}