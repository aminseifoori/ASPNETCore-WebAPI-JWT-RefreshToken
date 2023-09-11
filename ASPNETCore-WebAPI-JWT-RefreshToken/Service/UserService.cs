using ASPNETCore_WebAPI_JWT_RefreshToken.Models;
using ASPNETCore_WebAPI_JWT_RefreshToken.ServiceInterface;
using ASPNETCore_WebAPI_JWT_RefreshToken.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Dtos.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ASPNETCore_WebAPI_JWT_RefreshToken.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private User? user;
        public UserService(UserManager<User> _userManager, IMapper _mapper,
            IConfiguration _configuration)
        {
            userManager = _userManager;
            mapper = _mapper;
            configuration = _configuration;
        }
        public async Task<IdentityResult> RegisterUser(CreateUserDto createUserDto)
        {
            var user = mapper.Map<User>(createUserDto);

            var result = await userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "USER");
            }

            return result;
        }

        public async Task<IdentityResult> RegisterAdmin(CreateUserDto createUserDto)
        {
            var user = mapper.Map<User>(createUserDto);

            var result = await userManager.CreateAsync(user, createUserDto.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "ADMIN");
            }

            return result;
        }

        public async Task<bool> ValidateUser(LoginDto userForAuth)
        {
            user = await userManager.FindByNameAsync(userForAuth.UserName);
            var result = (user != null && await userManager.CheckPasswordAsync(user, userForAuth.Password));
            return result;
        }

        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            if (populateExp)
            {
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(3);
            }
            await userManager.UpdateAsync(user);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var _user = await userManager.FindByNameAsync(principal.Identity.Name);
            if (_user == null || _user.RefreshToken != tokenDto.RefreshToken || _user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception("Invalid client request. The tokenDto has some invalid values.");
            user = _user;
            return await CreateToken(populateExp: false);
        }

        private SigningCredentials GetSigningCredentials()
        {
            JwtSettings serviceSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(serviceSettings.JwtSecters);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            JwtSettings serviceSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            var tokenOptions = new JwtSecurityToken
                (
                    issuer: serviceSettings.ValidIssuer,
                    audience: serviceSettings.ValidAudience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(serviceSettings.Expires)),
                    signingCredentials: signingCredentials
                );
            return tokenOptions;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            JwtSettings serviceSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceSettings.JwtSecters)),
                ValidateLifetime = true,
                ValidIssuer = serviceSettings.ValidIssuer,
                ValidAudience = serviceSettings.ValidAudience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
