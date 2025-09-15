using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Profiles;

/// <summary>
/// Profile class for mapping Passenger-related DTOs to domain entities and vice versa.
/// </summary>
public class PassengerProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PassengerProfile"/> class.
    /// Configures mappings between Passenger-related DTOs and domain entities.
    /// </summary>
    public PassengerProfile()
    {
        CreateMap<CreatePassengerDto, Passenger>()
            .ConstructUsing(dto => new Passenger(
                    dto.UserId,
                new PaymentDetails(
                    dto.CardNumber,
                    dto.ExpirationDate,
                    dto.Cvv)
                )
            );
        
        CreateMap<Passenger, ResponsePassenger>().ReverseMap();
        
        // PaymentDetails mapping
        CreateMap<UpdatePaymentDetailsDto, PaymentDetails>();

        CreateMap<PaymentDetails, ResponsePaymentDetails>().ReverseMap();
    }
}
