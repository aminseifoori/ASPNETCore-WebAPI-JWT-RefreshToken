using Microsoft.AspNetCore.Identity;
using Shared.Dtos.Users;

namespace ASPNETCore_WebAPI_JWT_RefreshToken.ServiceInterface
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(CreateUserDto createUserDto);
        Task<bool> ValidateUser(LoginDto userForAuth);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
