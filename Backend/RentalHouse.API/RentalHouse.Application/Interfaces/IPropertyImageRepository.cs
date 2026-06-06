using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IPropertyImageRepository : IRepository<PropertyImage>
{
    Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId);
}