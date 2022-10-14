using JwtRefreshToken.Models.Entities;
using System.Security.Claims;

namespace JwtRefreshToken.Models.Repositories;

public interface IJWTManagerRepository
{
	Token GenerateToken(string userName);
	Token GenerateRefreshToken(string userName);
	ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
