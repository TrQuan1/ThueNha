using Microsoft.EntityFrameworkCore;
using RentalHouse.Application.Interfaces;
using RentalHouse.Domain.Entities;
using RentalHouse.Infrastructure.Data;

namespace RentalHouse.Infrastructure.Repositories;

public class PropertyImageRepository : GenericRepository<PropertyImage>, IPropertyImageRepository
{
    public PropertyImageRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId)
    {
        return await _dbSet
            .Where(pi => pi.PropertyId == propertyId)
            .OrderByDescending(pi => pi.CreatedAt)
            .ToListAsync();
    }
}