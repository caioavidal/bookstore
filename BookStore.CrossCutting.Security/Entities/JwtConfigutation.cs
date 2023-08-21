namespace BookStore.CrossCutting.Security.Entities;

public record JwtConfiguration(string Issuer, string Audience, string Key);