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
/// Service responsible for handling passenger-related operations.
/// </summary>
public class PassengerService : IPassengerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public PassengerService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<ResponsePassenger>> RegisterPassenger(CreatePassengerDto dto)
    {
        var passenger = _mapper.Map<Passenger>(dto);
        _unitOfWork.Passengers.AddPassenger(passenger);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponsePassenger>.Failure(CommonErrors.CommitedFailed());

        var passengerCreated = await _unitOfWork.Passengers.GetPassengerByIdAsync(passenger.UserId);
        var response = _mapper.Map<ResponsePassenger>(passengerCreated);
        
        var cacheKey = CacheKeys.Passenger.ById(passenger.UserId);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "passenger:list*");
        return Result<ResponsePassenger>.Success(response);
    }

    public async Task<Result<IEnumerable<ResponsePassenger>>> GetAllPassengers()
    {
        var cacheKey = CacheKeys.Passenger.List();
        var cachedPassengers = await _cache.GetAsync<IEnumerable<ResponsePassenger>>(cacheKey);
        if (cachedPassengers != null)
            return Result<IEnumerable<ResponsePassenger>>.Success(cachedPassengers);

        var passengers = await _unitOfWork.Passengers.GetAllPassengersAsync();
        if (!passengers.Any())
            return Result<IEnumerable<ResponsePassenger>>.Failure(PassengerErrors.PassengerNotFound());

        var response = _mapper.Map<IEnumerable<ResponsePassenger>>(passengers).ToList();
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<IEnumerable<ResponsePassenger>>.Success(response);
    }

    public async Task<Result<ResponsePassenger>> GetPassenger(int id)
    {
        var cacheKey = CacheKeys.Passenger.ById(id);
        var cachedPassenger = await _cache.GetAsync<ResponsePassenger>(cacheKey);
        if (cachedPassenger != null)
            return Result<ResponsePassenger>.Success(cachedPassenger);

        var passenger = await _unitOfWork.Passengers.GetPassengerByIdAsync(id);
        if (passenger == null)
            return Result<ResponsePassenger>.Failure(PassengerErrors.PassengerKeyNotFound());

        var response = _mapper.Map<ResponsePassenger>(passenger);
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponsePassenger>.Success(response);
    }

    public async Task<Result> UpdatePaymentDetails(int id, UpdatePaymentDetailsDto dto)
    {
        var existingPassenger = await _unitOfWork.Passengers.GetPassengerByIdAsync(id);
        if (existingPassenger == null)
            return Result.Failure(PassengerErrors.PassengerKeyNotFound());
        
        var paymentDetails = _mapper.Map<PaymentDetails>(dto);
        existingPassenger.UpdatePaymentInformation(paymentDetails);
        
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());
        
        var response = _mapper.Map<ResponsePassenger>(existingPassenger);

        var cacheKey = CacheKeys.Passenger.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        await _cache.RemoveByPatternAsync(pattern: "passenger:list*");
        return Result.Success();
    }

    public async Task<Result> DeletePassenger(int id)
    {
        var passenger = await _unitOfWork.Passengers.GetPassengerByIdAsync(id);
        if (passenger == null)
            return Result.Failure(PassengerErrors.PassengerKeyNotFound());

        _unitOfWork.Passengers.DeletePassenger(passenger);
        passenger.User.SetRole("Pending");

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var cacheKeyPassenger = CacheKeys.Passenger.ById(id);
        var cacheKeyUser = CacheKeys.User.ById(id);
        await _cache.RemoveAsync(cacheKeyPassenger);
        await _cache.RemoveAsync(cacheKeyUser);
        await _cache.RemoveByPatternAsync(pattern: "passenger:list*");
        return Result.Success();
    }
}
