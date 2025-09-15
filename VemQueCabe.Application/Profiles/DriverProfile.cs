using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Profiles;

/// <summary>
/// Profile configuration for mapping Driver-related DTOs and entities.
/// </summary>
public class DriverProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DriverProfile"/> class.
    /// Configures mappings between Driver-related DTOs and entities.
    /// </summary>
    public DriverProfile()
    {
        CreateMap<CreateDriverDto, Driver>()
            .ConstructUsing(dto => new Driver(
                    dto.UserId,
                new Vehicle(
                    dto.Brand,
                    dto.Model,
                    dto.Year,
                    dto.Color,
                    dto.Plate)
                )
            );

        CreateMap<Driver, ResponseDriver>().ReverseMap();
        
        // Vehicle 
        CreateMap<UpdateVehicleDto, Vehicle>();
    }
}
