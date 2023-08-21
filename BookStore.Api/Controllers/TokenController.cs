using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.Application.Contracts;
using BookStore.Application.Dtos.Input;
using BookStore.Application.Entities;
using BookStore.CrossCutting.Security.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.Api.Controllers;

public class TokenController : ApiController
{
    private readonly IApplicationUserRepository _applicationUserRepository;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IPasswordHasher _passwordHasher;

    public TokenController(JwtConfiguration jwtConfiguration,
        IApplicationUserRepository applicationUserRepository,
        IPasswordHasher passwordHasher)
    {
        _jwtConfiguration = jwtConfiguration;
        _applicationUserRepository = applicationUserRepository;
        _passwordHasher = passwordHasher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateToken([FromBody] LoginDto login)
    {
        IActionResult response = Unauthorized();
        var user = await AuthenticateUser(login);

        if (user is null) return response;

        var tokenString = GenerateJsonWebToken(user);
        response = Ok(new { token = tokenString });

        return response;
    }

    private string GenerateJsonWebToken(ApplicationUser userInfo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim("role", userInfo.Role),
            new Claim(JwtRegisteredClaimNames.Sid, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<ApplicationUser> AuthenticateUser(LoginDto loginDto)
    {
        var user = await _applicationUserRepository.GetByEmail(loginDto.Email);

        if (user is null) return null;

        var isValid = _passwordHasher.Verify(loginDto.Password, user.PasswordHash, user.Salt);

        return isValid ? user : null;
    }
}