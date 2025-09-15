using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Enums;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.Shared.Extensions;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Service for managing ride-related operations.
/// </summary>
public class RideService : IRideService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideRequestService _rideRequestService;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public RideService(IUnitOfWork unitOfWork, IMapper mapper, IRideRequestService rideRequestService, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _rideRequestService = rideRequestService;
        _cache = cache;
    }

    public async Task<Result<ResponseRide>> CreateRide(CreateRideDto dto)
    {
        var driver = await _unitOfWork.Drivers.GetDriverByIdAsync(dto.DriverId);
        if (driver == null)
            return Result<ResponseRide>.Failure(DriverErrors.DriverKeyNotFound());
        if (!driver.IsAvailable)
            return Result<ResponseRide>.Failure(DriverErrors.DriverNotAvailable());

        var rideRequest = await _unitOfWork.RideRequests.GetRequestByIdAsync(dto.RideRequestId);
        if (rideRequest == null)
            return Result<ResponseRide>.Failure(RideRequestErrors.RideRequestKeyNotFound());
        if (rideRequest.Status != Status.Pending)
            return Result<ResponseRide>.Failure(CommonErrors.InvalidOperation("Ride request is not in a valid state to start a ride."));

        var ride = _mapper.Map<Ride>(dto);
        
        ride.Fare.CalculateTotal(rideRequest.Distance);
        driver.SetAvailability(false);

        await _rideRequestService.UpdateRideRequestStatus(dto.RideRequestId, Status.InProgress);

        _unitOfWork.Rides.CreateRide(ride);
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseRide>.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseRide>(ride);
        
        var cacheKey = CacheKeys.Ride.ById(ride.RideId);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "ride:list*");
        return Result<ResponseRide>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseRide>>> GetAllRides()
    {
        var cacheKey = CacheKeys.Ride.List();
        var cachedRides = await _cache.GetAsync<IEnumerable<ResponseRide>>(cacheKey);
        if (cachedRides != null)
            return Result<IEnumerable<ResponseRide>>.Success(cachedRides);

        var rides = await _unitOfWork.Rides.GetAllRidesAsync();
        if (!rides.Any())
            return Result<IEnumerable<ResponseRide>>.Failure(RideErrors.RideNotFound());

        var response = _mapper.Map<IEnumerable<ResponseRide>>(rides).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(3));
        return Result<IEnumerable<ResponseRide>>.Success(response);
    }

    public async Task<Result<ResponseRide>> GetRideById(int id)
    {
        var cacheKey = CacheKeys.Ride.ById(id);
        var cachedRide = await _cache.GetAsync<ResponseRide>(cacheKey);
        if (cachedRide != null)
            return Result<ResponseRide>.Success(cachedRide);

        var ride = await _unitOfWork.Rides.GetRideByIdAsync(id);
        if (ride == null)
            return Result<ResponseRide>.Failure(RideErrors.RideKeyNotFound());

        var response = _mapper.Map<ResponseRide>(ride);
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseRide>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseRide>>> GetRidesByDriverId(int id)
    {
        var cacheKey = CacheKeys.Ride.ActiveByDriverId(id);
        var cachedRides = await _cache.GetAsync<IEnumerable<ResponseRide>>(cacheKey);
        if (cachedRides != null)
            return Result<IEnumerable<ResponseRide>>.Success(cachedRides);

        var rides = await _unitOfWork.Rides.GetRidesByDriverIdAsync(id);
        if (!rides.Any())
            return Result<IEnumerable<ResponseRide>>.Failure(RideErrors.RideNotFoundForThisDriver());

        var response = _mapper.Map<IEnumerable<ResponseRide>>(rides).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponseRide>>.Success(response);
    }

    public async Task<Result> SetEndRide(int id)
    {
        var ride = await _unitOfWork.Rides.GetRideByIdAsync(id);
        if (ride == null)
            return Result.Failure(RideErrors.RideKeyNotFound());

        ride.EndRide();
        await _rideRequestService.UpdateRideRequestStatus(ride.RideRequestId, Status.Completed);
        
        ride.Driver.SetAvailability(true);
        ride.RideRequest.Passenger.SetHasActiveRequest(false);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseRide>(ride);
        
        var cacheKey = CacheKeys.Ride.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "ride:list*");
        return Result.Success();
    }

    public async Task<Result> DeleteRide(int id)
    {
        var ride = await _unitOfWork.Rides.GetRideByIdAsync(id);
        if (ride == null)
            return Result.Failure(RideErrors.RideKeyNotFound());
        
        if (ride.RideRequest.Status != Status.Completed)
            return Result.Failure(CommonErrors.InvalidOperation("Cannot delete a ride that is in progress."));
        
        _unitOfWork.Rides.DeleteRide(ride);
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var cacheKey = CacheKeys.Ride.ById(id);
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveByPatternAsync(pattern: "ride:list*");
        return Result.Success();
    }
}
