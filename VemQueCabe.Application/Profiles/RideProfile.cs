using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Profiles;

/// <summary>
/// Profile configuration for mapping Ride-related DTOs and entities.
/// </summary>
public class RideProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RideProfile"/> class.
    /// Configures mappings between DTOs and domain entities for Ride-related operations.
    /// </summary>
    public RideProfile()
    {
        CreateMap<CreateRideDto, Ride>()
            .ConstructUsing(dto => new Ride(
                dto.RideRequestId,
                dto.DriverId,
                new Fare(dto.BaseFare, dto.SurgeMultiplier)
            ));
        
        CreateMap<Ride, ResponseRide>().ReverseMap();
        
        // Fare
        CreateMap<Fare, ResponseFare>().ReverseMap();
    }
}
