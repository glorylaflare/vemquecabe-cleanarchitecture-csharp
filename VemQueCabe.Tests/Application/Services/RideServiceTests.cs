using Moq;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class RideServiceTests : RideServiceFixture
{
    [Fact]
    public async Task CreateRide_ShouldReturnRide_WhenDataIsValid()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var requestId = dto.RideRequestId;
        var driverId = dto.DriverId;
        var ride = GenerateRide(dto);
        var driver = GenerateDriver(driverId);
        var request = GenerateRideRequest(requestId);
        var status = Status.InProgress;
        var cacheKey = CacheKeys.Ride.ById(ride.RideId);
        var response = CreateResponseRide(ride);

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(requestId)).ReturnsAsync(request);
        _mockMap.Setup(m => m.Map<Ride>(dto)).Returns(ride);
        _mockRequest.Setup(r => r.UpdateRideRequestStatus(requestId, status));
        _mockRepo.Setup(r => r.Rides.CreateRide(ride));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseRide>(ride)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("ride:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(ride.RideId, result.Value.RideId);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(requestId), Times.Once);
        _mockMap.Verify(m => m.Map<Ride>(dto), Times.Once);
        _mockRequest.Verify(r => r.UpdateRideRequestStatus(requestId, status), Times.Once);
        _mockRepo.Verify(r => r.Rides.CreateRide(ride), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseRide>(ride), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("ride:list*"), Times.Once);
    }

    [Fact]
    public async Task CreateRide_ShouldReturnError_WhenDriverNotFound()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var driverId = dto.DriverId;
        
        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync((Driver?)null);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Driver with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRide_ShouldReturnError_WhenDriverIsNotAvailable()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var driverId = dto.DriverId;
        var driver = GenerateDriver(driverId);
        driver.SetAvailability(false);
        
        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync(driver);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("The driver is not available.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRide_ShouldReturnError_WhenRideRequestNotFound()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var requestId = dto.RideRequestId;
        var driverId = dto.DriverId;
        var driver = GenerateDriver(driverId);

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(requestId)).ReturnsAsync((RideRequest?)null);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(requestId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRide_ShouldReturnError_WhenRideRequestIsNotPending()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var requestId = dto.RideRequestId;
        var driverId = dto.DriverId;
        var driver = GenerateDriver(driverId);
        var request = GenerateRideRequest(requestId);
        request.UpdateStatus(Status.Completed);

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(requestId)).ReturnsAsync(request);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request is not in a valid state to start a ride.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(requestId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRide_ShouldReturnError_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var requestId = dto.RideRequestId;
        var driverId = dto.DriverId;
        var ride = GenerateRide(dto);
        var driver = GenerateDriver(driverId);
        var request = GenerateRideRequest(requestId);
        var status = Status.InProgress;

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(driverId)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(requestId)).ReturnsAsync(request);
        _mockMap.Setup(m => m.Map<Ride>(dto)).Returns(ride);
        _mockRequest.Setup(r => r.UpdateRideRequestStatus(requestId, status));
        _mockRepo.Setup(r => r.Rides.CreateRide(ride));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().CreateRide(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);

        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(driverId), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(requestId), Times.Once);
        _mockMap.Verify(m => m.Map<Ride>(dto), Times.Once);
        _mockRequest.Verify(r => r.UpdateRideRequestStatus(requestId, status), Times.Once);
        _mockRepo.Verify(r => r.Rides.CreateRide(ride), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllRides_ShouldReturnRides_WhenTheyExist()
    {
        // Arrange
        var key = CacheKeys.Ride.List();
        var rideList = GenerateList(() => GenerateRide(GenerateCreateRideDto())).ToList();
        var responseRides = rideList
            .Select(CreateResponseRide)
            .ToList();
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync((IEnumerable<ResponseRide>?)null);
        _mockRepo.Setup(r => r.Rides.GetAllRidesAsync()).ReturnsAsync(rideList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseRide>>(rideList)).Returns(responseRides);
        _mockCache.Setup(c => c.SetAsync(key, responseRides, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetAllRides();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseRides, result.Value);
        Assert.Equal(rideList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.Rides.GetAllRidesAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseRide>>(rideList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseRide>>(s => s.SequenceEqual(responseRides)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAllRides_ShouldReturnError_WhenNoRidesExist()
    {
        // Arrange
        var key = CacheKeys.Ride.List();
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync((IEnumerable<ResponseRide>?)null);
        _mockRepo.Setup(r => r.Rides.GetAllRidesAsync()).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetAllRides();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No rides found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetAllRidesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task GetAllRides_ShouldReturnRidesFromCache_WhenTheyExistInCache()
    {
        // Arrange
        var key = CacheKeys.Ride.List();
        var rideList = GenerateList(() => GenerateRide(GenerateCreateRideDto())).ToList();
        var responseRides = rideList
            .Select(CreateResponseRide)
            .ToList();
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync(responseRides);
        
        // Act
        var result = await CreateService().GetAllRides();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseRides, result.Value);
        Assert.Equal(rideList.Count, result.Value.Count());
    }
    
    [Fact]
    public async Task GetRideById_ShouldReturnRide_WhenItExists()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        var responseRide = CreateResponseRide(ride);
        var key = CacheKeys.Ride.ById(id);
        
        _mockCache.Setup(c => c.GetAsync<ResponseRide>(key)).ReturnsAsync((ResponseRide?)null);
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        _mockMap.Setup(m => m.Map<ResponseRide>(ride)).Returns(responseRide);
        _mockCache.Setup(c => c.SetAsync(key, responseRide, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetRideById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(responseRide, result.Value);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseRide>(ride), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, responseRide, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetRideById_ShouldReturnError_WhenItDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.Ride.ById(id);
        
        _mockCache.Setup(c => c.GetAsync<ResponseRide>(key)).ReturnsAsync((ResponseRide?)null);
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync((Ride?)null);
        
        // Act
        var result = await CreateService().GetRideById(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetRideById_ShouldReturnRideFromCache_WhenItExistsInCache()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        var responseRide = CreateResponseRide(ride);
        var key = CacheKeys.Ride.ById(id);
        
        _mockCache.Setup(c => c.GetAsync<ResponseRide>(key)).ReturnsAsync(responseRide);
        
        // Act
        var result = await CreateService().GetRideById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseRide, result.Value);
        Assert.Equal(id, result.Value.RideId);
    }

    [Fact]
    public async Task GetRidesByDriverId_ShouldReturnRides_WhenTheyExist()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var rideList = GenerateList(() => GenerateRide(dto)).ToList();
        var responseRides = rideList
            .Select(CreateResponseRide)
            .ToList();
        var id = dto.DriverId;
        var key = CacheKeys.Ride.ActiveByDriverId(id);
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync((IEnumerable<ResponseRide>?)null);
        _mockRepo.Setup(r => r.Rides.GetRidesByDriverIdAsync(id)).ReturnsAsync(rideList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseRide>>(rideList)).Returns(responseRides);
        _mockCache.Setup(c => c.SetAsync(key, responseRides, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetRidesByDriverId(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseRides, result.Value);
        Assert.Equal(rideList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.Rides.GetRidesByDriverIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseRide>>(rideList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseRide>>(s => s.SequenceEqual(responseRides)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetRidesByDriverId_ShouldReturnError_WhenNoRidesExist()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var id = dto.DriverId;
        var key = CacheKeys.Ride.ActiveByDriverId(id);
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync((IEnumerable<ResponseRide>?)null);
        _mockRepo.Setup(r => r.Rides.GetRidesByDriverIdAsync(id)).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetRidesByDriverId(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No rides found for this driver.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRidesByDriverIdAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task GetRidesByDriverId_ShouldReturnRidesFromCache_WhenTheyExistInCache()
    {
        // Arrange
        var dto = GenerateCreateRideDto();
        var rideList = GenerateList(() => GenerateRide(dto)).ToList();
        var responseRides = rideList
            .Select(CreateResponseRide)
            .ToList();
        var id = dto.DriverId;
        var key = CacheKeys.Ride.ActiveByDriverId(id);
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRide>>(key)).ReturnsAsync(responseRides);
        
        // Act
        var result = await CreateService().GetRidesByDriverId(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseRides, result.Value);
        Assert.Equal(rideList.Count, result.Value.Count());
        
        _mockCache.Verify(c => c.GetAsync<IEnumerable<ResponseRide>>(key), Times.Once);
    }
    
    [Fact]
    public async Task SetEndRide_ShouldReturnSuccess_WhenRideIsEndedSuccessfully()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        var requestId = ride.RideRequestId;
        var status = Status.Completed;
        ride.Driver.SetAvailability(false);
        ride.RideRequest.Passenger.SetHasActiveRequest(true);
        var response = CreateResponseRide(ride);
        var cacheKey = CacheKeys.Ride.ById(id);
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        _mockRequest.Setup(r => r.UpdateRideRequestStatus(requestId, status));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseRide>(ride)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("ride:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().SetEndRide(id);

        // Assert
        Assert.True(result.IsSuccess);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRequest.Verify(r => r.UpdateRideRequestStatus(requestId, status), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseRide>(ride), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("ride:list*"), Times.Once);
    }

    [Fact]
    public async Task SetEndRide_ShouldReturnError_WhenRideDoesNotExist()
    {
        // Arrange
        const int id = 1;
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync((Ride?)null);
        
        // Act
        var result = await CreateService().SetEndRide(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task SetEndRide_ShouldReturnError_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        var requestId = ride.RideRequestId;
        var status = Status.Completed;
        ride.Driver.SetAvailability(false);
        ride.RideRequest.Passenger.SetHasActiveRequest(true);
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        _mockRequest.Setup(r => r.UpdateRideRequestStatus(requestId, status));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().SetEndRide(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRequest.Verify(r => r.UpdateRideRequestStatus(requestId, status), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteRide_ShouldReturnSuccess_WhenRideIsDeletedSuccessfully()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        var cacheKey = CacheKeys.Ride.ById(id);
        ride.RideRequest.UpdateStatus(Status.Completed);
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        _mockRepo.Setup(r => r.Rides.DeleteRide(ride));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockCache.Setup(c => c.RemoveAsync(cacheKey)).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("ride:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().DeleteRide(id);

        // Assert
        Assert.True(result.IsSuccess);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Rides.DeleteRide(ride), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync(cacheKey), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("ride:list*"), Times.Once);
    }

    [Fact]
    public async Task DeleteRide_ShouldReturnError_WhenRideDoesNotExist()
    {
        // Arrange
        const int id = 1;
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync((Ride?)null);
        
        // Act
        var result = await CreateService().DeleteRide(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    
    [Fact]
    public async Task DeleteRide_ShouldReturnError_WhenRideRequestIsNotCompleted()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        ride.RideRequest.UpdateStatus(Status.InProgress);
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        
        // Act
        var result = await CreateService().DeleteRide(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Cannot delete a ride that is in progress.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteRide_ShouldReturnError_WhenCommitFails()
    {
        // Arrange
        const int id = 1;
        var ride = GenerateRide(GenerateCreateRideDto(), id);
        ride.RideRequest.UpdateStatus(Status.Completed);
        
        _mockRepo.Setup(r => r.Rides.GetRideByIdAsync(id)).ReturnsAsync(ride);
        _mockRepo.Setup(r => r.Rides.DeleteRide(ride));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().DeleteRide(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Rides.GetRideByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Rides.DeleteRide(ride), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
}