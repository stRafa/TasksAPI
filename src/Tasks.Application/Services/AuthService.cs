using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tasks.API.Configuration;
using Tasks.Application.DTOs.Identity;
using Tasks.Application.DTOs.User;
using Tasks.Application.DTOs.User.Validators;
using Tasks.Application.Interfaces;
using Tasks.Application.ViewModels;
using Tasks.Domain.User;

namespace Tasks.Application.Services;

public class AuthService : IAuthService
{
    
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly AppSettings _appSettings;

    public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IUserService userService, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
    }
    
    public async Task<ResultViewModel<LoginViewModel>> Register(CreateUserDTO userDto)
    {
        var validationResult = await new CreateUserDTOValidator().ValidateAsync(userDto);

        if (!validationResult.IsValid)
            return new ResultViewModel<LoginViewModel>(validationResult);
        
        var identityUser = new IdentityUser
        {
            Email = userDto.Email,
            UserName = userDto.Email,
            EmailConfirmed = true
        };
        var identityResult = await _userManager.CreateAsync(identityUser, userDto.Password);
        
        if (!identityResult.Succeeded)
        {
            return new ResultViewModel<LoginViewModel>(identityResult.Errors.Select(p => p.Description).ToList());
        }

        _userService.Create(userDto, Guid.Parse(identityUser.Id));

        var response = new LoginViewModel(await GenerateJwt(identityUser.Email));
        
        return new ResultViewModel<LoginViewModel>(response);
    }

    public async Task<ResultViewModel<LoginViewModel>> Login(LoginDTO loginDto)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, true);

        if (!signInResult.Succeeded)
            return new ResultViewModel<LoginViewModel>("Username or password incorrect");

        var response = new LoginViewModel(await GenerateJwt(loginDto.Username));
        
        return new ResultViewModel<LoginViewModel>(response);
    }
    
    
    private async Task<string> GenerateJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await GetUserClaims(claims, user);
            var encodedToken = EncodeToken(identityClaims);

            return encodedToken;
        }

        private async Task<ClaimsIdentity> GetUserClaims(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToUnixEpochDate().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixEpochDate().ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }
        
        
}

public static class DataTimeUtils
{
    public static long ToUnixEpochDate(this DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
}