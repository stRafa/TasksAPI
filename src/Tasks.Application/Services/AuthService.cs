using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tasks.API.Configuration;
using Tasks.Application.DTOs.Identity;
using Tasks.Application.DTOs.User;
using Tasks.Application.DTOs.User.Validators;
using Tasks.Application.Interfaces;
using Tasks.Application.ViewModels;
using Tasks.Domain.Passport;
using Tasks.Domain.Utils;
using Claim = System.Security.Claims.Claim;

namespace Tasks.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppSettings _appSettings;
    private readonly string _pepper;
    private readonly int _iteration = 3;
    private readonly IPassportRepository _passportRepository;

    public AuthService(IConfiguration configuration, IPassportRepository passportRepository)
    {
        _passportRepository = passportRepository;
        _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        _pepper = configuration.GetValue<string>("Pepper");
    }

    public async Task<ResultViewModel<LoginViewModel>> Register(CreateUserDTO userDto)
    {
        var validationResult = await new CreateUserDTOValidator().ValidateAsync(userDto);

        if (!validationResult.IsValid)
            return new ResultViewModel<LoginViewModel>(validationResult);

        var passwordSalt = PasswordHasher.GenerateSalt();
        var passwordHash = PasswordHasher.ComputeHash(userDto.Password, passwordSalt, _pepper, _iteration);

        var passport = new Passport(userDto.Name, userDto.Email, passwordHash, passwordSalt);

        _passportRepository.CreatePassport(passport);

        var response = new LoginViewModel(await GenerateJwt(passport));

        return new ResultViewModel<LoginViewModel>(response);
    }

    public async Task<ResultViewModel<LoginViewModel>> Login(LoginDTO loginDto)
    {
        var passport = await _passportRepository.GetPassportByUsername(loginDto.Username);

        if (passport is null)
            return new ResultViewModel<LoginViewModel>("Username or password incorrect");

        var passwordHash = PasswordHasher.ComputeHash(loginDto.Password, passport.PasswordSalt, _pepper, _iteration);

        if (passwordHash.Trim() != passport.PasswordHash.Trim())
            return new ResultViewModel<LoginViewModel>("Username or password incorrect");

        var response = new LoginViewModel(await GenerateJwt(passport));

        return new ResultViewModel<LoginViewModel>(response);
    }

    private async Task<string> GenerateJwt(Passport passport)
    {
        var identityClaims = await GetUserClaims(passport.Claims, passport);
        var encodedToken = EncodeToken(identityClaims);

        return encodedToken;
    }

    private async Task<ClaimsIdentity> GetUserClaims(List<Claim> claims, Passport user)
    {
        if (claims is null)
            return new ClaimsIdentity();

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Username));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToUnixEpochDate().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixEpochDate().ToString(),
            ClaimValueTypes.Integer64));
        foreach (var userRole in user.Roles)
        {
            claims.Add(new Claim("role", userRole.Name));
        }

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        return identityClaims;
    }

    private string EncodeToken(ClaimsIdentity identityClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _appSettings.Issuer,
            Audience = _appSettings.Audience,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(_appSettings.ExpireInHours),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        return tokenHandler.WriteToken(token);
    }
}

public static class DataTimeUtils
{
    public static long ToUnixEpochDate(this DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .TotalSeconds);
}