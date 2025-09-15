using AutoMapper;
using Bogus;
using Moq;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Application.Services;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Application.Fixtures;

public abstract class UserServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IMapper> _mockMap = new();
    protected readonly Mock<IPasswordService> _mockPassword = new();
    protected readonly Mock<ICacheService> _mockCache = new();
    protected readonly Mock<ITokenService> _mockToken = new();

    protected CreateUserDto GenerateCreateUserDto()
    {
        return new Faker<CreateUserDto>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password())
            .Generate(); 
    }

    private UpdateNameDto GenerateUpdateNameDto()
    {
        return new Faker<UpdateNameDto>()
            .RuleFor(n => n.FirstName, f => f.Name.FirstName())
            .RuleFor(n => n.LastName, f => f.Name.LastName())
            .Generate();
    }
    
    private UpdatePhoneNumberDto GenerateUpdatePhoneNumberDto()
    {
        return new Faker<UpdatePhoneNumberDto>()
            .RuleFor(p => p.CountryCode, f => f.Random.Int(1, 99).ToString())
            .RuleFor(p => p.Number, f => f.Phone.PhoneNumber())
            .Generate();
    }
    
    protected UpdateEmailDto GenerateUpdateEmailDto()
    {
        return new Faker<UpdateEmailDto>()
            .RuleFor(e => e.Address, f => f.Internet.Email())
            .Generate();
    }
    
    protected Email GenerateEmail(UpdateEmailDto dto)
    {
        return new Email(dto.Address);
    }
    
    protected UpdateUserDto GenerateUpdateUserDto()
    {
        return new Faker<UpdateUserDto>()
            .RuleFor(u => u.Name, GenerateUpdateNameDto())
            .RuleFor(u => u.PhoneNumber, GenerateUpdatePhoneNumberDto())
            .Generate();
    }

    protected ResponseUser GenerateResponseUser(User user)
    {
        return new Faker<ResponseUser>()
            .RuleFor(u => u.UserId, user.UserId)
            .RuleFor(u => u.Name, user.Name)
            .RuleFor(u => u.Email, user.Email)
            .RuleFor(u => u.PhoneNumber, user.PhoneNumber)
            .RuleFor(u => u.Role, user.Role)
            .Generate();
    }
    
    protected User GenerateUser(CreateUserDto dto, int? userId = null)
    {
        return new Faker<User>()
            .CustomInstantiator(_ => new User(
                new Name(dto.FirstName, dto.LastName),
                new Email(dto.Email), 
                new Password(dto.Password)))
            .RuleFor(u => u.UserId, f => userId ?? f.Random.Int(1, 1000))
            .Generate();
    }
    
    protected UserService CreateService() => 
        new(_mockRepo.Object, _mockMap.Object, _mockPassword.Object, _mockCache.Object, _mockToken.Object);
}