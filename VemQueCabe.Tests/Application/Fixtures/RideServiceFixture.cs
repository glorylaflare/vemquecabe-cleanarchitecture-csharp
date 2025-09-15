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

namespace VemQueCabe.Tests.Application.Fixtures;

public class RideServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IMapper> _mockMap = new();  
    protected readonly Mock<ICacheService> _mockCache = new();
    protected readonly Mock<IRideRequestService> _mockRequest = new();
    private readonly Faker _faker = new();

    protected CreateRideDto GenerateCreateRideDto()
    {
        return new Faker<CreateRideDto>()
            .RuleFor(r => r.RideRequestId, f => f.Random.Int(1, 1000))
            .RuleFor(r => r.DriverId, f => f.Random.Int(1, 1000))
            .RuleFor(r => r.BaseFare, f => f.Random.Decimal(5, 50))
            .RuleFor(r => r.SurgeMultiplier, f => f.Random.Decimal(1, 3))
            .Generate();
    }
    
    protected Driver GenerateDriver(int driverId)
    {
        return new Faker<Driver>()
            .CustomInstantiator(f => new Driver(
                driverId,
                new Vehicle(
                    f.Vehicle.Manufacturer(),
                    f.Vehicle.Model(),
                    f.Random.Int(1990, 2025),
                    f.Commerce.Color(),
                    f.Random.Replace("???#?##").ToUpper())
            ))
            .RuleFor(d => d.UserId, driverId)
            .Generate();
    }
    
    private Passenger GeneratePassenger()
    {
        return new Faker<Passenger>()
            .CustomInstantiator(f => new Passenger(
                f.Random.Int(1, 1000),
                new PaymentDetails(
                    "4242424242424242",
                    DateOnly.FromDateTime(f.Date.Future()),
                    f.Finance.CreditCardCvv())))
            .RuleFor(p => p.UserId, f => f.Random.Int(1, 1000))
            .Generate();
    }
    
    protected RideRequest GenerateRideRequest(int requestId)
    {
        return new Faker<RideRequest>()
            .CustomInstantiator(f => new RideRequest(
                requestId,
                f.Address.StreetAddress(),
                f.Address.StreetAddress(),
                f.Random.Decimal(1, 100),
                f.Random.Bool() ? f.Lorem.Sentence() : null))
            .RuleFor(rr => rr.RequestId, requestId)
            .RuleFor(rr => rr.Passenger, GeneratePassenger())
            .RuleFor(rr => rr.Status, Status.Pending)
            .Generate();
    }
    
    private ResponseUser GenerateResponseUser(int driverId)
    {
        return new Faker<ResponseUser>()
            .RuleFor(u => u.UserId, driverId)
            .RuleFor(u => u.Name, new Name(new Faker().Name.FirstName(), new Faker().Name.LastName()))
            .RuleFor(u => u.Email, new Email(new Faker().Internet.Email()))
            .Generate();
    }

    private ResponseRideRequest CreateResponseRideRequest()
    {
        return new Faker<ResponseRideRequest>()
            .RuleFor(rr => rr.RequestId, _faker.Random.Int(1, 1000))
            .RuleFor(rr => rr.StartLocation, _faker.Address.StreetAddress())
            .RuleFor(rr => rr.EndLocation, _faker.Address.StreetAddress())
            .RuleFor(rr => rr.Distance, _faker.Random.Decimal(1, 100))
            .RuleFor(rr => rr.UserPreferences, _faker.Random.Bool() ? _faker.Lorem.Sentence() : null)
            .Generate();
    }
    
    private ResponseDriver CreateResponseDriver(Driver driver, bool? isAvailable = null)
    {
        return new Faker<ResponseDriver>()
            .RuleFor(d => d.User, GenerateResponseUser(driver.UserId))
            .RuleFor(d => d.Vehicle, driver.Vehicle)
            .RuleFor(d => d.IsAvailable, f => isAvailable ?? f.Random.Bool())
            .Generate();
    }
    
    private ResponseFare CreateResponseFare()
    {
        return new Faker<ResponseFare>()
            .RuleFor(f => f.Total, _faker.Random.Decimal(10, 200))
            .Generate();
    }
    
    protected ResponseRide CreateResponseRide(Ride ride)
    {
        return new Faker<ResponseRide>()
            .RuleFor(r => r.RideId, ride.RideId)
            .RuleFor(r => r.Driver, CreateResponseDriver(GenerateDriver(ride.DriverId)))
            .RuleFor(r => r.RideRequest, CreateResponseRideRequest())
            .RuleFor(r => r.StartTime, ride.StartTime)
            .RuleFor(r => r.EndTime, ride.EndTime ?? null)
            .RuleFor(r => r.Fare, CreateResponseFare())
            .Generate();
    }
    
    protected Ride GenerateRide(CreateRideDto dto, int? rideId = null)
    {
        return new Faker<Ride>()
            .CustomInstantiator(_ => new Ride(
                dto.RideRequestId,
                dto.DriverId,
                new Fare(dto.BaseFare, dto.SurgeMultiplier))
            )
            .RuleFor(r => r.Driver, GenerateDriver(dto.DriverId))
            .RuleFor(r => r.RideRequest, GenerateRideRequest(dto.RideRequestId))
            .RuleFor(r => r.RideId, f => rideId ?? f.Random.Int(1, 1000))
            .RuleFor(r => r.StartTime, f => f.Date.Recent())
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
    
    protected RideService CreateService() =>
        new(_mockRepo.Object, _mockMap.Object, _mockRequest.Object, _mockCache.Object);
}