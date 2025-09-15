using Moq;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.ValueObjects;
using VemQueCabe.Tests.Application.Fixtures;

namespace VemQueCabe.Tests.Application.Services;

public class DriverServiceTests : DriverServiceFixture
{
    [Fact]
    public async Task RegisterDriverAsync_ShouldRegisterDriver_WhenDataIsValid()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateCreateDriverDto();
        var driver = GenerateDriver(dto, id);
        var response = GenerateResponseDriver(driver);
        var plate = dto.Plate;
        var cacheKey = CacheKeys.Driver.ById(id);
        
        _mockRepo.Setup(r => r.Drivers.ExistsByPlateAsync(plate)).ReturnsAsync(false);
        _mockMap.Setup(m => m.Map<Driver>(dto)).Returns(driver);
        _mockRepo.Setup(r => r.Drivers.AddDriver(driver));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockMap.Setup(m => m.Map<ResponseDriver>(driver)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("driver:list*")).Returns(Task.CompletedTask);
        
        // Act
        var result = await CreateService().RegisterDriver(dto);
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.User.UserId, result.Value.User.UserId);
        Assert.Equal(response.Vehicle.Plate, result.Value.Vehicle.Plate);
        
        _mockRepo.Verify(r => r.Drivers.ExistsByPlateAsync(plate), Times.Once);
        _mockMap.Verify(m => m.Map<Driver>(dto), Times.Once);
        _mockRepo.Verify(r => r.Drivers.AddDriver(driver), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseDriver>(driver), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("driver:list*"), Times.Once);
    }

    [Fact]
    public async Task RegisterDriverAsync_ShouldReturnConflict_WhenDriverWithSamePlateExists()
    {
        // Arrange
        var dto = GenerateCreateDriverDto();
        var plate = dto.Plate;
        
        _mockRepo.Setup(r => r.Drivers.ExistsByPlateAsync(plate)).ReturnsAsync(true);
        
        // Act
        var result = await CreateService().RegisterDriver(dto);
        
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("An error occurred while trying to register the driver.", result.Error.Message);
        Assert.Equal(409, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.ExistsByPlateAsync(plate), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task RegisterDriverAsync_ShouldReturnError_WhenCommitFails()
    {
        // Arrange
        var dto = GenerateCreateDriverDto();
        var driver = GenerateDriver(dto);
        var plate = dto.Plate;

        _mockRepo.Setup(r => r.Drivers.ExistsByPlateAsync(plate)).ReturnsAsync(false);
        _mockMap.Setup(m => m.Map<Driver>(dto)).Returns(driver);
        _mockRepo.Setup(r => r.Drivers.AddDriver(driver));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);

        // Act
        var result = await CreateService().RegisterDriver(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.ExistsByPlateAsync(plate), Times.Once);
        _mockMap.Verify(m => m.Map<Driver>(dto), Times.Once);
        _mockRepo.Verify(r => r.Drivers.AddDriver(driver), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllDriversAsync_ShouldReturnAllDrivers()
    {
        // Arrange
        var key = CacheKeys.Driver.List();
        var driverList = GenerateList(() => GenerateDriver(GenerateCreateDriverDto())).ToList();
        var responseList = driverList
            .Select(d => GenerateResponseDriver(d))
            .ToList();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync((IEnumerable<ResponseDriver>?)null);
        _mockRepo.Setup(r => r.Drivers.GetAllDriversAsync()).ReturnsAsync(driverList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseDriver>>(driverList)).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

        // Act
        var result = await CreateService().GetAllDrivers();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList.Count(), result.Value.Count());
        
        _mockRepo.Verify(r => r.Drivers.GetAllDriversAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseDriver>>(driverList), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseDriver>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetAllDriversAsync_ShouldReturnNotFound_WhenNoDriversExist()
    {
        // Arrange
        var key = CacheKeys.Driver.List();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync((IEnumerable<ResponseDriver>?)null);
        _mockRepo.Setup(r => r.Drivers.GetAllDriversAsync()).ReturnsAsync([]);

        // Act
        var result = await CreateService().GetAllDrivers();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Driver not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetAllDriversAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllDriversAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
    {
        // Arrange
        var key = CacheKeys.Driver.List();
        var driverList = GenerateList(() => GenerateDriver(GenerateCreateDriverDto())).ToList();
        var responseList = driverList
            .Select(d => GenerateResponseDriver(d))
            .ToList();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync(responseList);

        // Act
        var result = await CreateService().GetAllDrivers();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
    }

    [Fact]
    public async Task GetAvailableDriversAsync_ShouldReturnAvailableDrivers()
    {
        // Arrange
        var key = CacheKeys.Driver.Available();
        var driverList = GenerateList(() => GenerateDriver(GenerateCreateDriverDto())).ToList();
        var responseList = driverList
            .Select(d => GenerateResponseDriver(d))
            .ToList();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync((IEnumerable<ResponseDriver>?)null);
        _mockRepo.Setup(r => r.Drivers.GetAvailableDriversAsync()).ReturnsAsync(driverList);
        _mockMap.Setup(m => m.Map<IEnumerable<ResponseDriver>>(It.IsAny<IEnumerable<Driver>>())).Returns(responseList);
        _mockCache.Setup(c => c.SetAsync(key, responseList, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

        // Act
        var result = await CreateService().GetAvailableDrivers();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
        
        _mockRepo.Verify(r => r.Drivers.GetAvailableDriversAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<IEnumerable<ResponseDriver>>(It.IsAny<IEnumerable<Driver>>()), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, 
            It.Is<IEnumerable<ResponseDriver>>(s => s.SequenceEqual(responseList)), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetAvailableDriversAsync_ShouldReturnNotFound_WhenNoAvailableDriversExist()
    {
        // Arrange
        var key = CacheKeys.Driver.Available();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync((IEnumerable<ResponseDriver>?)null);
        _mockRepo.Setup(r => r.Drivers.GetAvailableDriversAsync()).ReturnsAsync([]);

        // Act
        var result = await CreateService().GetAvailableDrivers();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No available drivers found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetAvailableDriversAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAvailableDriversAsync_ShouldReturnFromCache_WhenCacheIsAvailable()
    {
        // Arrange
        var key = CacheKeys.Driver.Available();
        var driverList = GenerateList(() => GenerateDriver(GenerateCreateDriverDto()));
        var responseList = driverList
            .Select(d => GenerateResponseDriver(d))
            .ToList();

        _mockCache.Setup(c => c.GetAsync<IEnumerable<ResponseDriver>>(key)).ReturnsAsync(responseList);

        // Act
        var result = await CreateService().GetAvailableDrivers();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(responseList.Count, result.Value.Count());
    }

    [Fact]
    public async Task GetDriver_ShouldReturnSuccess_WhenDriverIsFound()
    {
        // Arrange
        var driver = GenerateDriver(GenerateCreateDriverDto());
        var response = GenerateResponseDriver(driver);
        var id = driver.UserId;
        var key = CacheKeys.Driver.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseDriver>(key)).ReturnsAsync((ResponseDriver?)null);
        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockMap.Setup(m => m.Map<ResponseDriver>(driver)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(key, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);

        // Act
        var result = await CreateService().GetDriver(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.User.UserId, result.Value.User.UserId);
        Assert.Equal(response.Vehicle.Plate, result.Value.Vehicle.Plate);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseDriver>(driver), Times.Once);
        _mockCache.Verify(c => c.SetAsync(key, response, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetDriver_ShouldReturnNotFound_WhenDriverDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var key = CacheKeys.Driver.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseDriver>(key)).ReturnsAsync((ResponseDriver?)null);
        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync((Driver?)null);

        // Act
        var result = await CreateService().GetDriver(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Driver with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
    }

    [Fact]
    public async Task GetDriver_ShouldReturnFromCache_WhenCacheIsAvailable()
    {
        // Arrange
        var driver = GenerateDriver(GenerateCreateDriverDto());
        var response = GenerateResponseDriver(driver);
        var id = driver.UserId;
        var key = CacheKeys.Driver.ById(id);

        _mockCache.Setup(c => c.GetAsync<ResponseDriver>(key)).ReturnsAsync(response);

        // Act
        var result = await CreateService().GetDriver(id);

        // Assert 
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(response.User.UserId, result.Value.User.UserId);
        Assert.Equal(response.Vehicle.Plate, result.Value.Vehicle.Plate);
    }

    [Fact]
    public async Task UpdateVehicle_ShouldReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var dto = GenerateUpdateVehicleDto();
        var vehicle = GenerateVehicle(dto);
        const int id = 1;
        var driver = GenerateDriver(GenerateCreateDriverDto(), id);
        var plate = dto.Plate;
        var cacheKey = CacheKeys.Driver.ById(id);
        var response = GenerateResponseDriver(driver);

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.Drivers.ExistsByPlateAsync(plate)).ReturnsAsync(false);
        _mockMap.Setup(m => m.Map<Vehicle>(dto)).Returns(vehicle);
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockMap.Setup(m => m.Map<ResponseDriver>(driver)).Returns(response);
        _mockCache.Setup(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("driver:list*")).Returns(Task.CompletedTask);

        // Act
        var result = await CreateService().UpdateVehicle(id, dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Brand, driver.Vehicle.Brand);
        Assert.Equal(plate, driver.Vehicle.Plate);
        Assert.Equal(dto.Model, driver.Vehicle.Model);
        Assert.Equal(dto.Color, driver.Vehicle.Color);
        Assert.Equal(dto.Year, driver.Vehicle.Year);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Drivers.ExistsByPlateAsync(plate), Times.Once);
        _mockMap.Verify(m => m.Map<Vehicle>(dto), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockMap.Verify(m => m.Map<ResponseDriver>(driver), Times.Once);
        _mockCache.Verify(c => c.SetAsync(cacheKey, response, It.IsAny<TimeSpan>()), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("driver:list*"), Times.Once);
    }

    [Fact]
    public async Task UpdateVehicle_ShouldReturnNotFound_WhenDriverDoesNotExist()
    {
        // Arrange
        const int id = 1;
        var dto = GenerateUpdateVehicleDto();

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync((Driver?)null);

        // Act
        var result = await CreateService().UpdateVehicle(id, dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Driver not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateVehicle_ShouldReturnConflict_WhenPlateAlreadyExists()
    {
        // Arrange
        var dto = GenerateUpdateVehicleDto();
        const int id = 1;
        var driver = GenerateDriver(GenerateCreateDriverDto(), id);
        var plate = dto.Plate;

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.Drivers.ExistsByPlateAsync(plate)).ReturnsAsync(true);

        // Act
        var result = await CreateService().UpdateVehicle(id, dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("An error occurred while trying to register the driver.", result.Error.Message);
        Assert.Equal(409, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Drivers.ExistsByPlateAsync(plate), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }
    
    [Fact]
    public async Task DeleteDriver_ShouldReturnSuccess_WhenDriverIsDeleted()
    {
        // Arrange
        var driver = GenerateDriver(GenerateCreateDriverDto());
        var id = driver.UserId;
        var cacheKeyDriver = CacheKeys.Driver.ById(id);
        var cacheKeyUser = CacheKeys.User.ById(id);

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.Drivers.DeleteDriver(driver));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(true);
        _mockCache.Setup(c => c.RemoveAsync(cacheKeyDriver)).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveAsync(cacheKeyUser)).Returns(Task.CompletedTask);
        _mockCache.Setup(c => c.RemoveByPatternAsync("driver:list*")).Returns(Task.CompletedTask);

        // Act
        var result = await CreateService().DeleteDriver(id);

        // Assert
        Assert.True(result.IsSuccess);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Drivers.DeleteDriver(driver), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync(cacheKeyDriver), Times.Once);
        _mockCache.Verify(c => c.RemoveAsync(cacheKeyUser), Times.Once);
        _mockCache.Verify(c => c.RemoveByPatternAsync("driver:list*"), Times.Once);
    }

    [Fact]
    public async Task DeleteDriver_ShouldReturnNotFound_WhenDriverDoesNotExist()
    {
        // Arrange
        const int id = 1;

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync((Driver?)null);

        // Act
        var result = await CreateService().DeleteDriver(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Driver with ID not found.", result.Error.Message);
        Assert.Equal(404, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteDriver_ShouldReturnError_WhenCommitFails()
    {
        // Arrange
        var driver = GenerateDriver(GenerateCreateDriverDto());
        var id = driver.UserId;

        _mockRepo.Setup(r => r.Drivers.GetDriverByIdAsync(id)).ReturnsAsync(driver);
        _mockRepo.Setup(r => r.Drivers.DeleteDriver(driver));
        _mockRepo.Setup(r => r.CommitAsync()).ReturnsAsync(false);

        // Act
        var result = await CreateService().DeleteDriver(id);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Failed to commit changes to the database.", result.Error.Message);
        Assert.Equal(400, result.Error.Code);
        
        _mockRepo.Verify(r => r.Drivers.GetDriverByIdAsync(id), Times.Once);
        _mockRepo.Verify(r => r.Drivers.DeleteDriver(driver), Times.Once);
        _mockRepo.Verify(r => r.CommitAsync(), Times.Once);
    }
}