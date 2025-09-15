using AutoMapper;
using VemQueCabe.Application.Dtos;
using VemQueCabe.Application.Extensions;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Entities;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.Shared.Extensions;
using VemQueCabe.Domain.ValueObjects;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Service for managing user-related operations in the application.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;
    private readonly ICacheService _cache;
    private readonly ITokenService _tokenService;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordService passwordService, ICacheService cache, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
        _cache = cache;
        _tokenService = tokenService;
    }

    public async Task<Result<ResponseUser>> CreateUser(CreateUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
        if (existingUser != null)
            return Result<ResponseUser>.Failure(UserErrors.UserAlreadyExists());

        dto.Password = _passwordService.Hash(dto.Password);

        var user = _mapper.Map<User>(dto);

        _unitOfWork.Users.CreateUser(user);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseUser>.Failure(CommonErrors.CommitedFailed());
        
        var response = _mapper.Map<ResponseUser>(user);
        
        var cacheKey = CacheKeys.User.ById(user.UserId);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseUser>.Success(response);
    }
    
    public async Task<Result<ResponseUser>> GetUser(int id)
    {
        var cacheKey = CacheKeys.User.ById(id);
        var cachedUser = await _cache.GetAsync<ResponseUser>(cacheKey);
        if (cachedUser != null)
            return Result<ResponseUser>.Success(cachedUser);

        var user = await _unitOfWork.Users.GetUserByIdAsync(id);
        if (user == null)
            return Result<ResponseUser>.Failure(UserErrors.UserKeyNotFound());

        var response = _mapper.Map<ResponseUser>(user);
        
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseUser>.Success(response);
    }

    public async Task<Result> UpdateUser(int id, UpdateUserDto dto)
    {
        var existingUser = await _unitOfWork.Users.GetUserByIdAsync(id);
        if (existingUser == null)
            return Result.Failure(UserErrors.UserKeyNotFound());

        _mapper.Map(dto, existingUser);
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseUser>(existingUser);
        
        var cacheKey = CacheKeys.User.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result.Success();
    }

    public async Task<Result<ResponseAuth>> SetUserRole(int id, string role) 
    { 
        var existingUser = await _unitOfWork.Users.GetUserByIdAsync(id);
        if (existingUser == null)
            return Result<ResponseAuth>.Failure(UserErrors.UserKeyNotFound());

        existingUser.SetRole(role);
        var token = _tokenService.GenerateToken(existingUser);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseAuth>.Failure(CommonErrors.CommitedFailed());
        
        var response = _mapper.Map<ResponseUser>(existingUser);
        
        var cacheKey = CacheKeys.User.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result<ResponseAuth>.Success(new ResponseAuth
        {
            Email = existingUser.Email.Address,
            Role = existingUser.Role.ToString(),
            Token = token,
            RefreshToken = existingUser.Token.RefreshToken
        });
    }

    public async Task<Result> UpdateEmail(int id, UpdateEmailDto dto)
    {
        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(dto.Address);
        if (emailExists)
            return Result.Failure(UserErrors.EmailAlreadyInUse());

        var existingUser = await _unitOfWork.Users.GetUserByIdAsync(id);
        if (existingUser == null)
            return Result.Failure(UserErrors.UserNotFound());

        var email = _mapper.Map<Email>(dto);
        existingUser.UpdateEmailAddress(email);

        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result.Failure(CommonErrors.CommitedFailed());

        var response = _mapper.Map<ResponseUser>(existingUser);
        
        var cacheKey = CacheKeys.User.ById(id);
        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return Result.Success();
    }
}
