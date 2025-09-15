using Moq;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class RideRequestServiceTests : RideRequestServiceFixture
{
    [Fact]
    public async Task CreateRideRequestAsync_ShouldCreateRideRequest_WhenDataIsValid()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var passenger = rideRequest.Passenger;
        var response = CreateResponseRideRequest(rideRequest);
        var cacheKey = CacheKeys.RideRequest.ById(response.RequestId);

        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<RideRequest>(dto)).Returns(rideRequest);
        _mockRepo.Setup(r => r.RideRequests.AddRequest(rideRequest));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseRideRequest>(rideRequest)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("riderequest:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().CreateRideRequest(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(rideRequest.RequestId, result.Value.RequestId);
        Assert.Equal(rideRequest.Status.ToString(), result.Value.Status);
        Assert.True(passenger.HasActiveRequest);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId), Times.Once);
        _mockMap.Verify(m => m.Map<RideRequest>(dto), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.AddRequest(rideRequest), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseRideRequest>(rideRequest), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("riderequest:list*"), Times.Once);
    }

    [Fact]
    public async Task CreateRideRequestAsync_ShouldReturnFailure_WhenPassengerHasActiveRequest()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var passenger = rideRequest.Passenger;
        passenger.SetHasActiveRequest(true);

        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId)).ReturnsAsync(passenger);
        
        // Act
        var result = await CreateService().CreateRideRequest(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passenger already has an active ride request.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task CreateRideRequestAsync_ShouldReturnFailure_WhenPassengerDoesNotExist()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();

        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId)).ReturnsAsync((Passenger?)null);
        
        // Act
        var result = await CreateService().CreateRideRequest(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Passenger with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateRideRequestAsync_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var passenger = rideRequest.Passenger;

        _mockRepo.Setup(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId)).ReturnsAsync(passenger);
        _mockMap.Setup(m => m.Map<RideRequest>(dto)).Returns(rideRequest);
        _mockRepo.Setup(r => r.RideRequests.AddRequest(rideRequest));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().CreateRideRequest(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Passengers.GetPassengerByIdAsync(dto.PassengerId), Times.Once);
        _mockMap.Verify(m => m.Map<RideRequest>(dto), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.AddRequest(rideRequest), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllRideRequestsAsync_ShouldReturnAllRideRequests_WhenTheyExist()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequestList = GenerateList(() => GenerateRideRequest(dto)).ToList();
        var responseList = rideRequestList
            .Select(CreateResponseRideRequest)
            .ToList();
        var key = CacheKeys.RideRequest.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetAllRequestsAsync()).ReturnsAsync(rideRequestList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseRideRequest>>(rideRequestList)).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetAllRideRequests();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(rideRequestList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.RideRequests.GetAllRequestsAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseRideRequest>>(rideRequestList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseRideRequest>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }
    
    [Fact]
    public async Task GetAllRideRequestsAsync_ShouldReturnAllRideRequestsFromCache_WhenTheyExistInCache()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequestList = GenerateList(() => GenerateRideRequest(dto)).ToList();
        var responseList = rideRequestList
            .Select(CreateResponseRideRequest)
            .ToList();
        var key = CacheKeys.RideRequest.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync(responseList);
        
        // Act
        var result = await CreateService().GetAllRideRequests();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
    }
    
    [Fact]
    public async Task GetAllRideRequestsAsync_ShouldReturnFailure_WhenNoRideRequestsExist()
    {
        // Arrange
        var key = CacheKeys.RideRequest.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetAllRequestsAsync()).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetAllRideRequests();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No ride requests found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetAllRequestsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRideRequestByIdAsync_ShouldReturnRideRequest_WhenItExists()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var response = CreateResponseRideRequest(rideRequest);
        var id = rideRequest.RequestId;
        var key = CacheKeys.RideRequest.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseRideRequest>(key)).ReturnsAsync((ResponseRideRequest?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockMap.Setup(m => m.Map<ResponseRideRequest>(rideRequest)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(key, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetRideRequestById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(id, result.Value.RequestId);
        Assert.Equal(rideRequest.Status.ToString(), result.Value.Status);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseRideRequest>(rideRequest), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetRideRequestByIdAsync_ShouldReturnRideRequestFromCache_WhenItExistsInCache()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var response = CreateResponseRideRequest(rideRequest);
        var id = rideRequest.RequestId;
        var key = CacheKeys.RideRequest.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseRideRequest>(key)).ReturnsAsync(response);
        
        // Act
        var result = await CreateService().GetRideRequestById(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response, result.Value);
        Assert.Equal(id, result.Value.RequestId);
        Assert.Equal(rideRequest.Status.ToString(), result.Value.Status);
    }
    
    [Fact]
    public async Task GetRideRequestByIdAsync_ShouldReturnFailure_WhenItDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.RideRequest.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseRideRequest>(key)).ReturnsAsync((ResponseRideRequest?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync((RideRequest?)null);
        
        // Act
        var result = await CreateService().GetRideRequestById(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetRequestsByPassengerIdAsync_ShouldReturnRideRequest_WhenItExists()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequestList = GenerateList(() => GenerateRideRequest(dto)).ToList();
        var responseList = rideRequestList.
            Select(CreateResponseRideRequest)
            .ToList();
        var id = dto.PassengerId;
        var key = CacheKeys.RideRequest.ActiveByPassengerId(id);

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestsByPassengerIdAsync(id)).ReturnsAsync(rideRequestList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseRideRequest>>(rideRequestList)).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetRequestsByPassengerId(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(rideRequestList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestsByPassengerIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseRideRequest>>(rideRequestList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseRideRequest>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetRequestsByPassengerIdAsync_ShouldReturnRideRequestFromCache_WhenItExists()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var responseList = GenerateList(() => CreateResponseRideRequest(rideRequest)).ToList();
        var id = dto.PassengerId;
        var key = CacheKeys.RideRequest.ActiveByPassengerId(id);

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync(responseList);
        
        // Act
        var result = await CreateService().GetRequestsByPassengerId(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
    }
    
    [Fact]
    public async Task GetRequestsByPassengerIdAsync_ShouldReturnFailure_WhenItDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.RideRequest.ActiveByPassengerId(id);

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestsByPassengerIdAsync(id)).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetRequestsByPassengerId(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No ride requests found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestsByPassengerIdAsync(id), Times.Once);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    [InlineData("Canceled")]
    public async Task GetRequestsByStatusAsync_ShouldReturnRideRequest_WhenItExists(string status)
    {
        // Arrange
        const int count = 20;
        var dto = GenerateCreateRideRequestDto();
        var rideRequestList = GenerateList(() => GenerateRideRequest(dto), count);
        var filtedList = rideRequestList
            .Where(r => r.Status.ToString().Equals(status))
            .ToList();
        var responseList = filtedList
            .Select(CreateResponseRideRequest)
            .ToList();
        var key = CacheKeys.RideRequest.ActiveByStatus(status);
        var parsedStatus = Enum.Parse<Status>(status);

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestsByStatusAsync(parsedStatus)).ReturnsAsync(filtedList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseRideRequest>>(filtedList)).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().GetRequestsByStatus(parsedStatus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(filtedList.Count, result.Value.Count());
        Assert.All(result.Value, r => r.Status.ToString().Equals(status));
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestsByStatusAsync(parsedStatus), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseRideRequest>>(filtedList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseRideRequest>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    [InlineData("Canceled")]
    public async Task GetRequestsByStatusAsync_ShouldReturnRideRequestFromCache_WhenItExistsInCache(string status)
    {
        // Arrange
        const int count = 20;
        var dto = GenerateCreateRideRequestDto();
        var rideRequestList = GenerateList(() => GenerateRideRequest(dto), count);
        var filtedList = rideRequestList
            .Where(r => r.Status.ToString().Equals(status))
            .ToList();
        var responseList = filtedList
            .Select(CreateResponseRideRequest)
            .ToList();
        var key = CacheKeys.RideRequest.ActiveByStatus(status);
        var parsedStatus = Enum.Parse<Status>(status);
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync(responseList);
        
        // Act
        var result = await CreateService().GetRequestsByStatus(parsedStatus);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList, result.Value);
        Assert.Equal(filtedList.Count, result.Value.Count());
        Assert.All(result.Value, r => r.Status.ToString().Equals(status));
    }

    [Fact]
    public async Task GetRequestsByStatusAsync_ShouldReturnFailure_WhenItDoesNotExist()
    {
        // Arrange
        var status = Status.InProgress;
        var key = CacheKeys.RideRequest.ActiveByStatus(status.ToString());
        
        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseRideRequest>>(key)).ReturnsAsync((IEnumerable<ResponseRideRequest>?)null);
        _mockRepo.Setup(r => r.RideRequests.GetRequestsByStatusAsync(status)).ReturnsAsync([]);
        
        // Act
        var result = await CreateService().GetRequestsByStatus(status);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No ride requests found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestsByStatusAsync(status), Times.Once);
    }
    
    [Fact]
    public async Task UpdateRideRequestStatusAsync_ShouldUpdateStatus_WhenDataIsValid()
    {
        // Arrange
        var newStatus = Status.InProgress;
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        var cacheKey = CacheKeys.RideRequest.ById(id);
        var response = CreateResponseRideRequest(rideRequest);

        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseRideRequest>(rideRequest)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("riderequest:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().UpdateRideRequestStatus(id, newStatus);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStatus, rideRequest.Status);
        Assert.Equal(id, rideRequest.RequestId);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("riderequest:list*"), Times.Once);
    }
    
    [Fact]
    public async Task UpdateRideRequestStatusAsync_ShouldReturnFailure_WhenRideRequestDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var newStatus = Status.InProgress;

        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync((RideRequest?)null);
        
        // Act
        var result = await CreateService().UpdateRideRequestStatus(id, newStatus);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task UpdateRideRequestStatusAsync_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var newStatus = Status.InProgress;
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;

        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().UpdateRideRequestStatus(id, newStatus);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task CancelRideRequestAsync_ShouldCancelRideRequest_WhenItExistsAndIsPending()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        rideRequest.Passenger.SetHasActiveRequest(true);
        var cacheKey = CacheKeys.RideRequest.ById(id);
        var response = CreateResponseRideRequest(rideRequest);
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseRideRequest>(rideRequest)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("riderequest:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().CancelRideRequest(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Canceled, rideRequest.Status);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("riderequest:list*"), Times.Once);
    }
    
    [Fact]
    public async Task CancelRideRequestAsync_ShouldReturnFailure_WhenRideRequestDoesNotExist()
    {
        // Arrange
        const int id = 1;
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync((RideRequest?)null);
        
        // Act
        var result = await CreateService().CancelRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CancelRideRequestAsync_ShouldReturnFailure_WhenRideRequestIsNotPending()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        rideRequest.UpdateStatus(Status.InProgress);
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        
        // Act
        var result = await CreateService().CancelRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Only pending ride requests can be canceled.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task CancelRideRequestAsync_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        rideRequest.Passenger.SetHasActiveRequest(true);
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().CancelRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteRideRequestAsync_ShouldDeleteRideRequest_WhenItExists()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        rideRequest.UpdateStatus(Status.Canceled);
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.RideRequests.DeleteRequest(rideRequest));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockCache.Setup(c => c.RemoveByPatternAsync("riderequest:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().DeleteRideRequest(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, rideRequest.RequestId);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.DeleteRequest(rideRequest), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("riderequest:list*"), Times.Once);
    }

    [Fact]
    public async Task DeleteRideRequestAsync_ShouldReturnFailure_WhenRideRequestDoesNotExist()
    {
        // Arrange
        const int id = 1;
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync((RideRequest?)null);
        
        // Act
        var result = await CreateService().DeleteRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Ride request with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteRideRequestAsync_ShouldReturnFailure_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        rideRequest.UpdateStatus(Status.Canceled);
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        _mockRepo.Setup(r => r.RideRequests.DeleteRequest(rideRequest));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);
        
        // Act
        var result = await CreateService().DeleteRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.RideRequests.DeleteRequest(rideRequest), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
    
    [Fact]
    public async Task DeleteRideRequestAsync_ShouldReturnFailure_WhenRideRequestIsNotCanceled()
    {
        // Arrange
        var dto = GenerateCreateRideRequestDto();
        var rideRequest = GenerateRideRequest(dto);
        var id = rideRequest.RequestId;
        
        _mockRepo.Setup(r => r.RideRequests.GetRequestByIdAsync(id)).ReturnsAsync(rideRequest);
        
        // Act
        var result = await CreateService().DeleteRideRequest(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Cannot delete an active ride request.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.RideRequests.GetRequestByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
}