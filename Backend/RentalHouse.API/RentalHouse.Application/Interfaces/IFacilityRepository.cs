using RentalHouse.Domain.Entities;

namespace RentalHouse.Application.Interfaces;

public interface IFacilityRepository : IRepository<Facility>
{
    Task<IEnumerable<Facility>> GetFacilitiesByPropertyIdAsync(int propertyId);
    Task AssignFacilityAsync(PropertyFacility propertyFacility);
    Task RemoveFacilityAsync(PropertyFacility propertyFacility);
    Task<bool> HasFacilityAsync(int propertyId, int facilityId);
    Task<PropertyFacility?> GetPropertyFacilityAsync(int propertyId, int facilityId);
}