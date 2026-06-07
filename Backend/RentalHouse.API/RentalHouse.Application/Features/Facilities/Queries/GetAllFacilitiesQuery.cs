using MediatR;
using RentalHouse.Application.DTOs.Facilities;
using RentalHouse.Application.Interfaces;

namespace RentalHouse.Application.Features.Facilities.Queries;

// 1. Lấy tất cả
public class GetAllFacilitiesQuery : IRequest<IEnumerable<FacilityDto>> { }

public class GetAllFacilitiesQueryHandler : IRequestHandler<GetAllFacilitiesQuery, IEnumerable<FacilityDto>>
{
    private readonly IFacilityRepository _facilityRepository;

    public GetAllFacilitiesQueryHandler(IFacilityRepository facilityRepository)
    {
        _facilityRepository = facilityRepository;
    }

    public async Task<IEnumerable<FacilityDto>> Handle(GetAllFacilitiesQuery request, CancellationToken cancellationToken)
    {
        var facilities = await _facilityRepository.GetAllAsync();
        return facilities.Select(f => new FacilityDto { Id = f.Id, Name = f.Name, Icon = f.Icon });
    }
}

// 2. Lấy theo ID nhà
public class GetFacilitiesByPropertyQuery : IRequest<IEnumerable<FacilityDto>>
{
    public int PropertyId { get; set; }
}

public class GetFacilitiesByPropertyQueryHandler : IRequestHandler<GetFacilitiesByPropertyQuery, IEnumerable<FacilityDto>>
{
    private readonly IFacilityRepository _facilityRepository;

    public GetFacilitiesByPropertyQueryHandler(IFacilityRepository facilityRepository)
    {
        _facilityRepository = facilityRepository;
    }

    public async Task<IEnumerable<FacilityDto>> Handle(GetFacilitiesByPropertyQuery request, CancellationToken cancellationToken)
    {
        var facilities = await _facilityRepository.GetFacilitiesByPropertyIdAsync(request.PropertyId);
        return facilities.Select(f => new FacilityDto { Id = f.Id, Name = f.Name, Icon = f.Icon });
    }
}