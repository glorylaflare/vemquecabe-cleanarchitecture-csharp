using AutoMapper;
using Bogus;
using Moq;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Application.Services;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.ValueObjects;
using Name = VemQueCabe.Domain.ValueObjects.Name;

namespace VemQueCabe.Tests.Application.Fixtures;

public abstract class PassengerServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IMapper> _mockMap = new();  
    protected readonly Mock<ICacheService> _mockCache = new();
    private readonly Faker _faker = new();
    
    protected CreatePassengerDto GenerateCreatePassengerDto()
    {
        return new Faker<CreatePassengerDto>()
            .RuleFor(p => p.UserId, f => f.Random.Int(1, 1000))
            .RuleFor(p => p.CardNumber, "4242424242424242")
            .RuleFor(p => p.ExpirationDate, f => DateOnly.FromDateTime(f.Date.Future()))
            .RuleFor(p => p.Cvv, f => f.Finance.CreditCardCvv())
            .Generate();
    }

    protected UpdatePaymentDetailsDto GenerateUpdatePaymentDetailsDto()
    {
        return new Faker<UpdatePaymentDetailsDto>()
            .RuleFor(p => p.CardNumber, "4242424242424242")
            .RuleFor(p => p.ExpirationDate, f => DateOnly.FromDateTime(f.Date.Future()))
            .RuleFor(p => p.Cvv, f => f.Finance.CreditCardCvv())
            .Generate();
    }
    
    protected PaymentDetails GeneratePaymentDetails(UpdatePaymentDetailsDto dto)
    {
        return new PaymentDetails(dto.CardNumber, dto.ExpirationDate, dto.Cvv);
    }
    
    private ResponseUser GenerateResponseUser(int? userId = null)
    {
        return new Faker<ResponseUser>()
            .RuleFor(u => u.UserId, f => userId ?? f.Random.Int(1, 1000))
            .RuleFor(u => u.Name, new Name(_faker.Name.FirstName(), _faker.Name.LastName()))
            .RuleFor(u => u.Email, new Email(_faker.Internet.Email()))
            .RuleFor(u => u.PhoneNumber, new PhoneNumber("55", _faker.Phone.PhoneNumber("###########")))
            .RuleFor(u => u.Role, Role.Passenger)
            .Generate();
    }
    
    protected ResponsePaymentDetails GenerateResponsePaymentDetails()
    {
        return new Faker<ResponsePaymentDetails>()
            .RuleFor(p => p.CardNumber, "4242424242424242")
            .RuleFor(p => p.ExpirationDate, f => DateOnly.FromDateTime(f.Date.Future()))
            .RuleFor(p => p.CardHolderName, "Visa")
            .Generate();
    }
    
    protected ResponsePassenger GenerateResponsePassenger(Passenger passenger, bool? hasActiveRequest = null)
    {
        return new Faker<ResponsePassenger>()
            .RuleFor(p => p.User, GenerateResponseUser(passenger.UserId))
            .RuleFor(p => p.PaymentInformation, GenerateResponsePaymentDetails())
            .RuleFor(p => p.HasActiveRequest, f => hasActiveRequest ?? f.Random.Bool())
            .Generate();
    }
    
    protected Passenger GeneratePassenger(CreatePassengerDto dto, int? passengerId = null)
    {
        return new Faker<Passenger>()
            .CustomInstantiator(f => new Passenger(
                passengerId ?? dto.UserId,
                new PaymentDetails(dto.CardNumber, dto.ExpirationDate, dto.Cvv)
            ))
            .RuleFor(d => d.User, new Faker<User>()
                .CustomInstantiator(f => new User(
                    new Name(f.Name.FirstName(), f.Name.LastName()),
                    new Email(f.Internet.Email()),
                    new Password(f.Internet.Password())))
                .RuleFor(u => u.UserId, dto.UserId)
                .RuleFor(u => u.Role , Role.Passenger))
            .Generate();
    }
    
    protected IEnumerable<T> GenerateList<T>(Func<T> generator, int count = 5)
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            list.Add(generator());
        }
        return list;
    }
    
    protected PassengerService CreateService() => 
        new(_mockRepo.Object, _mockMap.Object, _mockCache.Object);
}