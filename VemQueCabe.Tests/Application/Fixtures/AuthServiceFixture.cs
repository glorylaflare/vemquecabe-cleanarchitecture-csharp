using Bogus;
using Moq;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Requests;
using VemQueCabe.Application.Services;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Tests.Application.Fixtures;

public abstract class AuthServiceFixture
{
    protected readonly Mock<IUnitOfWork> _mockRepo = new();
    protected readonly Mock<IPasswordService> _mockPassword = new();
    protected readonly Mock<ITokenService> _mockToken = new();
    private readonly Faker _faker = new();

    protected RequestLogin CreateRequestLogin()
    {
        return new Faker<RequestLogin>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password(12))
            .Generate();
    }
    
    protected User CreateUser(RequestLogin login, int? userId = null)
    {
        return new Faker<User>()
            .CustomInstantiator(_ => new User(
                new Name(_faker.Name.FirstName(), _faker.Name.LastName()),
                new Email(login.Email),
                new Password(login.Password)
            ))
            .RuleFor(u => u.UserId, f => userId ?? f.Random.Int(1, 1000))
            .RuleFor(u => u.Role, f => f.PickRandom<Role>())
            .Generate();
    }

    protected AuthService CreateService() =>
        new(_mockRepo.Object, _mockPassword.Object, _mockToken.Object);
}