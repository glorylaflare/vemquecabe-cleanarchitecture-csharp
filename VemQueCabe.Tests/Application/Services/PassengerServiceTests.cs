using Moq;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class PassengerServiceTests : PassengerServiceFixture
{
    [Fact]
    public async Task RegisterPassenger_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var dto = GenerateCreatePassengerDto();
        var id = dto.UserId;
        var passenger = GeneratePassenger(dto);
        var response = GenerateResponsePassenger(passenger);
        var cacheKey = CacheKeys.Passenger.ById(id);

        _mockMap.Setup(m => m.Map<Passenger>(dto)).Returns(passenger);
        _mockRepo.Setup(r => r.Passengers.AddPassenger(passenger));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<ResponsePassenger>(passenger)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("passenger:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().RegisterPassenger(dto);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(response.User, result.Value.User);
        Assert.Equal(response.PaymentInformation, result.Value.PaymentInformation);
        
        _mockMap.Verify(m => m.Map<Passenger>(dto), Times.Once);
        _mockRepo.Verify(r => r.Passengers.AddPassenger(passenger), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponsePassenger>(passenger), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("passenger:list*"), Times.Once);
    }

    [Fact]
    public async Task RegisterPassenger_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateCreatePassengerDto();
        var passenger = GeneratePassenger(dto, id);

        _mockMap.Setup(m => m.Map<Passenger>(dto)).Returns(passenger);
        _mockRepo.Setup(r => r.Passengers.AddPassenger(passenger));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().RegisterPassenger(dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockMap.Verify(m => m.Map<Passenger>(dto), Times.Once);
        _mockRepo.Verify(r => r.Passengers.AddPassenger(passenger), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPassengers_ShouldReturnSuccess_WhenPassengersExist()
    {
        // Arrange
        var passengerList = GenerateList(() => GeneratePassenger(GenerateCreatePassengerDto())).ToList();
        var responseList = passengerList
            .Select(p => GenerateResponsePassenger(p))
            .ToList();
        var key = CacheKeys.Passenger.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponsePassenger>>(key)).ReturnsAsync((IEnumerable<ResponsePassenger>?)null);
        _mockRepo.Setup(r => r.Passengers.GetAllPassengersAsync()).ReturnsAsync(passengerList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponsePassenger>>(passengerList)).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetAllPassengers();
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.Passengers.GetAllPassengersAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponsePassenger>>(passengerList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponsePassenger>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetAllPassengers_ShouldReturnFailure_WhenNoPassengersExist()
    {
        // Arrange
        var key = CacheKeys.Passenger.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponsePassenger>>(key)).ReturnsAsync((IEnumerable<ResponsePassenger>?)null);
        _mockRepo.Setup(r => r.Passengers.GetAllPassengersAsync()).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetAllPassengers();
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passengers not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetAllPassengersAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllPassengers_ShouldReturnSuccess_WhenCacheExists()
    {
        // Arrange
        var passengerList = GenerateList(() => GeneratePassenger(GenerateCreatePassengerDto())).ToList();
        var responseList = passengerList
            .Select(p => GenerateResponsePassenger(p))
            .ToList();
        var key = CacheKeys.Passenger.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponsePassenger>>(key)).ReturnsAsync(responseList);
        
        // Act
        var result = await CreateService().GetAllPassengers();
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
        
        _mockCache.Verify(c => c.GetAsync<IEnumerable<ResponsePassenger>>(key), Times.Once);
    }

    [Fact]
    public async Task GetPassenger_ShouldReturnSuccess_WhenPassengerExists()
    {
        // Arrange
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var response = GenerateResponsePassenger(passenger);
        var id = passenger.UserId;
        var key = CacheKeys.Passenger.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponsePassenger>(key)).ReturnsAsync((ResponsePassenger?)null);
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<ResponsePassenger>(passenger)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(key, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetPassenger(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(response.User, result.Value.User);
        Assert.Equal(response.PaymentInformation, result.Value.PaymentInformation);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponsePassenger>(passenger), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetPassenger_ShouldReturnFailure_WhenPassengerDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.Passenger.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponsePassenger>(key)).ReturnsAsync((ResponsePassenger?)null);
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync((Passenger?)null);
        
        // Act
        var result = await CreateService().GetPassenger(id);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passenger with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetPassenger_ShouldReturnSuccess_WhenCacheExists()
    {
        // Arrange
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var response = GenerateResponsePassenger(passenger);
        var id = passenger.UserId;
        var key = CacheKeys.Passenger.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponsePassenger>(key)).ReturnsAsync(response);
        
        // Act
        var result = await CreateService().GetPassenger(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(response.User, result.Value.User);
        Assert.Equal(response.PaymentInformation, result.Value.PaymentInformation);
    }

    [Fact]
    public async Task UpdatePaymentDetails_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var dto = GenerateUpdatePaymentDetailsDto();
        var payment = GeneratePaymentDetails(dto);
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var id = passenger.UserId;
        var cacheKey = CacheKeys.Passenger.ById(id);
        var response = GenerateResponsePassenger(passenger);
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<PaymentDetails>(dto)).Returns(payment);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponsePassenger>(passenger)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("passenger:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().UpdatePaymentDetails(id, dto);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.CardNumber, passenger.PaymentInformation.CardNumber);
        Assert.Equal(dto.ExpirationDate, passenger.PaymentInformation.ExpirationDate);
        Assert.Equal(dto.Cvv, passenger.PaymentInformation.Cvv);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<PaymentDetails>(dto), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponsePassenger>(passenger), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("passenger:list*"), Times.Once);
    }

    [Fact]
    public async Task UpdatePaymentDetails_ShouldReturnFailure_WhenPassengerDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdatePaymentDetailsDto();
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync((Passenger?)null);
        
        // Act
        var result = await CreateService().UpdatePaymentDetails(id, dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passenger with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdatePaymentDetails_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateUpdatePaymentDetailsDto();
        var payment = GeneratePaymentDetails(dto);
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var id = passenger.UserId;
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<PaymentDetails>(dto)).Returns(payment);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().UpdatePaymentDetails(id, dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<PaymentDetails>(dto), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeletePassenger_ShouldReturnSuccess_WhenPassengerExists()
    {
        // Arrange
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var id = passenger.UserId;
        var cacheKeyPassenger = CacheKeys.Passenger.ById(id);
        var cacheKeyUser = CacheKeys.User.ById(id);
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockRepo.Setup(r => r.Passengers.DeletePassenger(passenger));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockCache.Setup(c => c.RemoveAsync(cacheKeyPassenger)).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveAsync(cacheKeyUser)).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("passenger:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().DeletePassenger(id);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Passengers.DeletePassenger(passenger), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync(cacheKeyPassenger), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync(cacheKeyUser), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("passenger:list*"), Times.Once);
    }

    [Fact]
    public async Task DeletePassenger_ShouldReturnFailure_WhenPassengerDoesNotExist()
    {
        // Arrange
        const int id = 1;
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync((Passenger?)null);
        
        // Act
        var result = await CreateService().DeletePassenger(id);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passenger with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeletePassenger_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var passenger = GeneratePassenger(GenerateCreatePassengerDto());
        var id = passenger.UserId;
        
        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(id)).ReturnsAsync(passenger);
        _mockRepo.Setup(r => r.Passengers.DeletePassenger(passenger));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().DeletePassenger(id);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Passengers.DeletePassenger(passenger), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
}