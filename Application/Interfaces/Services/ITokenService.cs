using Domain.Entities;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromExpiredToken(string token);
}
