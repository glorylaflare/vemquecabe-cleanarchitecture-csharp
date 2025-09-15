using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Requests;
using VemQueCabe.Application.Responses;
using VemQueCabe.Domain.Shared;
using VemQueCabe.Domain.Shared.Extensions;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Provides authentication services, including user login and token generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork unitOfWork, IPasswordService passwordService, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<ResponseAuth>> AuthenticateAsync(RequestLogin login)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(login.Email);
        if (user == null || !_passwordService.Compare(login.Password, user.Password.Hashed))
            return Result<ResponseAuth>.Failure(AuthErrors.InvalidCredentials());

        var token = _tokenService.GenerateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.AssignRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseAuth>.Failure(CommonErrors.CommitedFailed());
        
        return Result<ResponseAuth>.Success(new ResponseAuth
        {
            Email = user.Email.Address,
            Role = user.Role.ToString(),
            Token = token,
            RefreshToken = refreshToken
        });
    }
    
    public async Task<Result<ResponseAuth>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetUserByRefreshToken(refreshToken);
        if (user == null)
            return Result<ResponseAuth>.Failure(UserErrors.UserNotFound());
        
        if (user.Token.RefreshToken != refreshToken || user.Token.ExpiresAt <= DateTime.UtcNow)
            return Result<ResponseAuth>.Failure(AuthErrors.InvalidCredentials());
        
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        
        user.AssignRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        var success = await _unitOfWork.CommitAsync();
        if (!success)
            return Result<ResponseAuth>.Failure(CommonErrors.CommitedFailed());
        
        return Result<ResponseAuth>.Success(new ResponseAuth
        {
            Email = user.Email.Address,
            Role = user.Role.ToString(),
            Token = _tokenService.GenerateToken(user),
            RefreshToken = newRefreshToken
        });
    }
}
