using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
{
    public FacilityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Facility>> GetFacilitiesByPropertyIdAsync(int propertyId)
    {
        return await _context.Set<PropertyFacility>()
            .Where(pf => pf.PropertyId == propertyId)
            .Include(pf => pf.Facility)
            .Select(pf => pf.Facility!)
            .ToListAsync();
    }

    public async Task AssignFacilityAsync(PropertyFacility propertyFacility)
    {
        await _context.Set<PropertyFacility>().AddAsync(propertyFacility);
    }

    public Task RemoveFacilityAsync(PropertyFacility propertyFacility)
    {
        _context.Set<PropertyFacility>().Remove(propertyFacility);
        return Task.CompletedTask;
    }

    public async Task<bool> HasFacilityAsync(int propertyId, int facilityId)
    {
        return await _context.Set<PropertyFacility>()
            .AnyAsync(pf => pf.PropertyId == propertyId && pf.FacilityId == facilityId);
    }

    public async Task<PropertyFacility?> GetPropertyFacilityAsync(int propertyId, int facilityId)
    {
        return await _context.Set<PropertyFacility>()
            .FirstOrDefaultAsync(pf => pf.PropertyId == propertyId && pf.FacilityId == facilityId);
    }
}