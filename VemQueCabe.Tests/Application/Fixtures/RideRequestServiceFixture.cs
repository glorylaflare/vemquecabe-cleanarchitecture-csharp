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

public abstract class RideRequestServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IMapper> _mockMap = new();  
    protected readonly Mock<ICacheService> _mockCache = new();
    
    protected CreateRideRequestDto GenerateCreateRideRequestDto()
    {
        return new Faker<CreateRideRequestDto>()
            .RuleFor(r => r.PassengerId, f => f.Random.Int(1, 1000))
            .RuleFor(r => r.StartLocation, f => f.Address.StreetAddress())
            .RuleFor(r => r.EndLocation, f => f.Address.StreetAddress())
            .RuleFor(r => r.Distance, f => f.Random.Decimal(1, 100))
            .RuleFor(r => r.UserPreferences, f => f.Random.Bool() ? f.Lorem.Sentence() : null)
            .Generate();
    }
    
    private Passenger GeneratePassenger(int passengerId, bool? hasActiveRequest = null)
    {
        return new Faker<Passenger>()
            .CustomInstantiator(f => new Passenger(
                passengerId, 
                new PaymentDetails("4242424242424242", 
                    DateOnly.FromDateTime(f.Date.Future()), 
                    f.Random.Number(100, 999).ToString())))
            .RuleFor(p => p.HasActiveRequest, f => hasActiveRequest ?? f.Random.Bool())
            .Generate();
    }
    
    protected ResponseRideRequest CreateResponseRideRequest(RideRequest rideRequest)
    {
        return new Faker<ResponseRideRequest>()
            .RuleFor(r => r.RequestId, rideRequest.RequestId)
            .RuleFor(r => r.StartLocation, rideRequest.StartLocation)
            .RuleFor(r => r.EndLocation, rideRequest.EndLocation)
            .RuleFor(r => r.Distance, rideRequest.Distance)
            .RuleFor(r => r.UserPreferences, rideRequest.UserPreferences)
            .RuleFor(r => r.Status, rideRequest.Status.ToString())
            .Generate();
    }

    protected RideRequest GenerateRideRequest(CreateRideRequestDto dto, int? requestId = null)
    {
        return new Faker<RideRequest>()
            .CustomInstantiator(_ => new RideRequest(
                dto.PassengerId, 
                dto.StartLocation, 
                dto.EndLocation, 
                dto.Distance, 
                dto.UserPreferences))
            .RuleFor(r => r.Passenger, GeneratePassenger(dto.PassengerId, false))
            .RuleFor(r => r.RequestId, f => requestId ?? f.Random.Int(1, 1000))
            .RuleFor(r => r.Status, Status.Pending)
            .Generate();
    }
    
    protected IEnumerable<T> GenerateList<T>(Func<T> generator, int count = 5)
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var item = generator();
            if (item is RideRequest rideRequest)
            {
                rideRequest.GetType().GetProperty("Status")?.SetValue(rideRequest, new Faker().PickRandom<Status>());
            }
            list.Add(item);
        }
        return list;
    }
    
    protected RideRequestService CreateService() =>
        new(_mockRepo.Object, _mockMap.Object, _mockCache.Object);
}