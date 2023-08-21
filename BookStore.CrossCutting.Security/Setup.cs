using System.Text;
using BookStore.Application.Contracts;
using BookStore.CrossCutting.Security.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.CrossCutting.Security;

public static class Setup
{
    public static IServiceCollection AddSecurity(this IServiceCollection serviceCollection,
        JwtConfiguration jwtConfiguration)
    {
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidAudience = jwtConfiguration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key))
                };
            });

        serviceCollection.AddSingleton<IPasswordHasher, PasswordHasher>();

        return serviceCollection;
    }
}