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
/// Service for managing ride requests, including creation, retrieval, updating, and deletion.
/// </summary>
public class RideRequestService : IRideRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public RideRequestService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<ResponseRideRequest>> CreateRideRequest(CreateRideRequestDto dto)
    {
        var passenger = await _unitOfWork.Passengers.GetPassengerByIdAsync(dto.PassengerId);
        if (passenger == null)
            return Result<ResponseRideRequest>.Failure(PassengerErrors.PassengerKeyNotFound());

        if (passenger.HasActiveRequest)
            return Result<ResponseRideRequest>.Failure(RideRequestErrors.PassengerHasExistingRequest());

        passenger.SetHasActiveRequest(true);
        var request = _mapper.Map<RideRequest>(dto);
        
        _unitOfWork.RideRequests.AddRequest(request);
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseRideRequest>.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseRideRequest>(request);
        
        var cacheKey = CacheKeys.RideRequest.ById(response.RequestId);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "riderequest:list*");
        return Result<ResponseRideRequest>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseRideRequest>>> GetAllRideRequests()
    {
        var cacheKey = CacheKeys.RideRequest.List();
        var cachedRequests = await _cache.GetAsync<IEnumerable<ResponseRideRequest>>(cacheKey);
        if (cachedRequests != null)
            return Result<IEnumerable<ResponseRideRequest>>.Success(cachedRequests);

        var requests = await _unitOfWork.RideRequests.GetAllRequestsAsync();
        if (!requests.Any())
            return Result<IEnumerable<ResponseRideRequest>>.Failure(RideRequestErrors.RideRequestNotFound());

        var response = _mapper.Map<IEnumerable<ResponseRideRequest>>(requests).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponseRideRequest>>.Success(response);
    }

    public async Task<Result<ResponseRideRequest>> GetRideRequestById(int id)
    {
        var cacheKey = CacheKeys.RideRequest.ById(id);
        var cachedRequest = await _cache.GetAsync<ResponseRideRequest>(cacheKey);
        if (cachedRequest != null)
            return Result<ResponseRideRequest>.Success(cachedRequest);

        var request = await _unitOfWork.RideRequests.GetRequestByIdAsync(id);
        if (request == null)
            return Result<ResponseRideRequest>.Failure(RideRequestErrors.RideRequestKeyNotFound());

        var response = _mapper.Map<ResponseRideRequest>(request);
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseRideRequest>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseRideRequest>>> GetRequestsByPassengerId(int id)
    {
        var cacheKey = CacheKeys.RideRequest.ActiveByPassengerId(id);
        var cachedRequests = await _cache.GetAsync<IEnumerable<ResponseRideRequest>>(cacheKey);
        if (cachedRequests != null)
            return Result<IEnumerable<ResponseRideRequest>>.Success(cachedRequests);

        var requests = await _unitOfWork.RideRequests.GetRequestsByPassengerIdAsync(id);
        if (!requests.Any())
            return Result<IEnumerable<ResponseRideRequest>>.Failure(RideRequestErrors.RideRequestNotFound());

        var response = _mapper.Map<IEnumerable<ResponseRideRequest>>(requests).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponseRideRequest>>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponseRideRequest>>> GetRequestsByStatus(Status status)
    {
        var cacheKey = CacheKeys.RideRequest.ActiveByStatus(status.ToString());
        var cachedRequests = await _cache.GetAsync<IEnumerable<ResponseRideRequest>>(cacheKey);
        if (cachedRequests != null)
            return Result<IEnumerable<ResponseRideRequest>>.Success(cachedRequests);

        var requests = await _unitOfWork.RideRequests.GetRequestsByStatusAsync(status);
        if (!requests.Any())
            return Result<IEnumerable<ResponseRideRequest>>.Failure(RideRequestErrors.RideRequestNotFound());

        var response = _mapper.Map<IEnumerable<ResponseRideRequest>>(requests).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponseRideRequest>>.Success(response);
    }

    public async Task<Result> UpdateRideRequestStatus(int id, Status status)
    {
        var request = await _unitOfWork.RideRequests.GetRequestByIdAsync(id);
        if (request == null)
            return Result.Failure(RideRequestErrors.RideRequestKeyNotFound());

        request.UpdateStatus(status);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());
        
        var response = _mapper.Map<ResponseRideRequest>(request);
        
        var cacheKey = CacheKeys.RideRequest.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "riderequest:list*");
        return Result.Success();
    }

    public async Task<Result> CancelRideRequest(int id)
    {
        var request = await _unitOfWork.RideRequests.GetRequestByIdAsync(id);
        if (request == null)
            return Result.Failure(RideRequestErrors.RideRequestKeyNotFound());

        if (request.Status != Status.Pending)
            return Result.Failure(RideRequestErrors.CannotCancelNonPendingRequest());

        request.UpdateStatus(Status.Canceled);
        request.Passenger.SetHasActiveRequest(false);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseRideRequest>(request);
        
        var cacheKey = CacheKeys.RideRequest.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "riderequest:list*");
        return Result.Success();
    }

    public async Task<Result> DeleteRideRequest(int id)
    {
        var request = await _unitOfWork.RideRequests.GetRequestByIdAsync(id);
        if (request == null)
            return Result.Failure(RideRequestErrors.RideRequestKeyNotFound());
        
        if (request.Status != Status.Canceled)
            return Result.Failure(RideRequestErrors.CannotDeleteActiveRequest());

        _unitOfWork.RideRequests.DeleteRequest(request);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var cacheKey = CacheKeys.RideRequest.ById(id);
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveByPatternAsync(pattern: "riderequest:list*");
        return Result.Success();
    }
}
