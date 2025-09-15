using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;

namespace VemQueCabe.Application.Profiles;

/// <summary>
/// Profile class for mapping RideRequest-related DTOs to domain entities and vice versa.
/// </summary>
public class RideRequestProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RideRequestProfile"/> class.
    /// Configures mappings between CreateRideRequestDto and RideRequest, and between RideRequest and ResponseRideRequestDto.
    /// </summary>
    public RideRequestProfile()
    {
        CreateMap<CreateRideRequestDto, RideRequest>()
            .ConstructUsing(dto => new RideRequest(
                    dto.PassengerId,
                    dto.StartLocation,
                    dto.EndLocation,
                    dto.Distance,
                    dto.UserPreferences
                ));

        CreateMap<RideRequest, ResponseRideRequest>().ReverseMap();
    }
}
