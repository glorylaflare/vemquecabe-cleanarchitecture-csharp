using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.Shared.Extensions;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Service implementation for managing drivers and their associated operations.
/// </summary>
public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public DriverService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<ResponseDriver>> RegisterDriver(CreateDriverDto dto)
    {
        var existingPlate = await _unitOfWork.Drivers.ExistsByPlateAsync(dto.Plate);
        if (existingPlate)
            return Result<ResponseDriver>.Failure(DriverErrors.DriverAlreadyExists());

        var driver = _mapper.Map<Driver>(dto);
        _unitOfWork.Drivers.AddDriver(driver);
        
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseDriver>.Failure(CommonErrors.CommitedFailed());

        var driverCreated = await _unitOfWork.Drivers.GetDriverByIdAsync(driver.UserId);
        var response = _mapper.Map<ResponseDriver>(driverCreated);
        
        var cacheKey = CacheKeys.Driver.ById(driver.UserId);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "driver:list*");
        return Result<ResponseDriver>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseDriver>>> GetAllDrivers()
    {
        var cachekey = CacheKeys.Driver.List();
        var cachedDrivers = await _cache.GetAsync<IEnumerable<ResponseDriver>>(cachekey);
        if (cachedDrivers != null)
            return Result<IEnumerable<ResponseDriver>>.Success(cachedDrivers);

        var drivers = await _unitOfWork.Drivers.GetAllDriversAsync();
        if (!drivers.Any())
            return Result<IEnumerable<ResponseDriver>>.Failure(DriverErrors.DriverNotFound());

        var response = _mapper.Map<IEnumerable<ResponseDriver>>(drivers).ToList();
        
        await _cache.SetAsync(cachekey, response, TimeSpan.FromMinutes(3));
        return Result<IEnumerable<ResponseDriver>>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseDriver>>> GetAvailableDrivers()
    {
        var cachekey = CacheKeys.Driver.Available();
        var cachedDrivers = await _cache.GetAsync<IEnumerable<ResponseDriver>>(cachekey);
        if (cachedDrivers != null)
            return Result<IEnumerable<ResponseDriver>>.Success(cachedDrivers);

        var drivers = await _unitOfWork.Drivers.GetAvailableDriversAsync();
        if (!drivers.Any())
            return Result<IEnumerable<ResponseDriver>>.Failure(DriverErrors.DriversNoAvailableFound());

        var response = _mapper.Map<IEnumerable<ResponseDriver>>(drivers).ToList();
        
        await _cache.SetAsync(cachekey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponseDriver>>.Success(response);
    }

    public async Task<Result<ResponseDriver>> GetDriver(int id)
    {
        var cachekey = CacheKeys.Driver.ById(id);
        var cachedDriver = await _cache.GetAsync<ResponseDriver>(cachekey);
        if (cachedDriver != null)
            return Result<ResponseDriver>.Success(cachedDriver);

        var driver = await _unitOfWork.Drivers.GetDriverByIdAsync(id);
        if (driver == null)
            return Result<ResponseDriver>.Failure(DriverErrors.DriverKeyNotFound());

        var response = _mapper.Map<ResponseDriver>(driver);
        
        await _cache.SetAsync(cachekey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseDriver>.Success(response);
    }

    public async Task<Result> UpdateVehicle(int id, UpdateVehicleDto dto)
    {
        var existingDriver = await _unitOfWork.Drivers.GetDriverByIdAsync(id);
        if (existingDriver == null) 
            return Result.Failure(DriverErrors.DriverNotFound());

        var existingPlate = await _unitOfWork.Drivers.ExistsByPlateAsync(dto.Plate);
        if (existingPlate)
            return Result.Failure(DriverErrors.DriverAlreadyExists());

        var vehicle = _mapper.Map<Vehicle>(dto);
        existingDriver.UpdateVehicle(vehicle);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseDriver>(existingDriver);
        
        var cacheKey = CacheKeys.Driver.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "driver:list*");
        return Result.Success();
    }

    public async Task<Result> DeleteDriver(int id)
    {
        var driver = await _unitOfWork.Drivers.GetDriverByIdAsync(id);
        if (driver == null)
            return Result.Failure(DriverErrors.DriverKeyNotFound());

        _unitOfWork.Drivers.DeleteDriver(driver);
        driver.User.SetRole("Pending");

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var cacheKeyDriver = CacheKeys.Driver.ById(id);
        var cacheKeyUser = CacheKeys.User.ById(id);
        await _cache.RemoveAsync(cacheKeyDriver);
        await _cache.RemoveAsync(cacheKeyUser);
        await _cache.RemoveByPatternAsync(pattern: "driver:list*");
        return Result.Success();
    }
}
