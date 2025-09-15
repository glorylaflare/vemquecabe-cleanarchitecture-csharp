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

public abstract class DriverServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IMapper> _mockMap = new();  
    protected readonly Mock<ICacheService> _mockCache = new();
    private readonly Faker _faker = new();
    
    protected CreateDriverDto GenerateCreateDriverDto()
    {
        return new Faker<CreateDriverDto>()
            .RuleFor(d => d.UserId, f => f.Random.Int(1, 1000))
            .RuleFor(d => d.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(d => d.Model, f => f.Vehicle.Model())
            .RuleFor(d => d.Year, f => f.Random.Int(1990, 2025))
            .RuleFor(d => d.Color, f => f.Commerce.Color())
            .RuleFor(d => d.Plate, f => f.Random.Replace("???#?##").ToUpper())
            .Generate();
    }
    
    protected UpdateVehicleDto GenerateUpdateVehicleDto()
    {
        return new Faker<UpdateVehicleDto>()
            .RuleFor(d => d.Brand, f => f.Vehicle.Manufacturer())
            .RuleFor(d => d.Model, f => f.Vehicle.Model())
            .RuleFor(d => d.Year, f => f.Random.Int(1990, 2025))
            .RuleFor(d => d.Color, f => f.Commerce.Color())
            .RuleFor(d => d.Plate, f => f.Random.Replace("???#?##").ToUpper())
            .Generate();
    }
    
    protected Vehicle GenerateVehicle(UpdateVehicleDto dto)
    {
        return new Vehicle(dto.Brand, dto.Model, dto.Year, dto.Color, dto.Plate);
    }
    
    private ResponseUser GenerateResponseUser(int? userId = null)
    {
        return new Faker<ResponseUser>()
            .RuleFor(u => u.UserId, f => userId ?? f.Random.Int(1, 1000))
            .RuleFor(u => u.Name, new Name(_faker.Name.FirstName(), _faker.Name.LastName()))
            .RuleFor(u => u.Email, new Email(_faker.Internet.Email()))
            .RuleFor(u => u.PhoneNumber, new PhoneNumber("55", _faker.Phone.PhoneNumber("###########")))
            .RuleFor(u => u.Role, Role.Driver)
            .Generate();
    }
    
    protected ResponseDriver GenerateResponseDriver(Driver driver, bool? isAvailable = null)
    {
        return new Faker<ResponseDriver>()
            .RuleFor(d => d.User, GenerateResponseUser(driver.UserId))
            .RuleFor(d => d.Vehicle, driver.Vehicle)
            .RuleFor(d => d.IsAvailable, f => isAvailable ?? f.Random.Bool())
            .Generate();
    }

    protected Driver GenerateDriver(CreateDriverDto dto, int? driverId = null)
    {
        return new Faker<Driver>()
            .CustomInstantiator(f => new Driver(
                driverId ?? dto.UserId,
                new Vehicle(dto.Brand, dto.Model, dto.Year, dto.Color, dto.Plate)
            ))
            .RuleFor(d => d.User, new Faker<User>()
                .CustomInstantiator(f => new User(
                    new Name(f.Name.FirstName(), f.Name.LastName()),
                    new Email(f.Internet.Email()),
                    new Password(f.Internet.Password())))
                .RuleFor(u => u.UserId, dto.UserId)
                .RuleFor(u => u.Role , Role.Driver))
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

    protected DriverService CreateService() =>
        new(_mockRepo.Object, _mockMap.Object, _mockCache.Object);
}