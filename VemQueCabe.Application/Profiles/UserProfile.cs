using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Profiles;

/// <summary>
/// Profile configuration for mapping between User-related DTOs and the User entity.
/// </summary>
public class UserProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfile"/> class.
    /// Configures mappings for CreateUserDto, UpdateUserDto, and ResponseUserDto.
    /// </summary>
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>()
            .ConstructUsing(dto => new User(
                new Name(dto.FirstName, dto.LastName),
                new Email(dto.Email),
                new Password(dto.Password)
            ));

        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opts =>
                opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<User, ResponseUser>().ReverseMap();
        
        // Email
        CreateMap<UpdateEmailDto, Email>();
        
        // Name
        CreateMap<UpdateNameDto, Name>();
        
        // PhoneNumber
        CreateMap<UpdatePhoneNumberDto, PhoneNumber>();
    }
}
