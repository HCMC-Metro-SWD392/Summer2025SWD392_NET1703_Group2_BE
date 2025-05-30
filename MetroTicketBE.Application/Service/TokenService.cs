using MetroTicket.Domain.Entities;
using MetroTicketBE.Application.IService;
using MetroTicketBE.Infrastructure.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MetroTicketBE.Application.Service
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRedisService _redisService;
        public TokenService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IRedisService redisService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
        }
        public async Task<string> GenerateJwtAccessTokenAsync(ApplicationUser user)
        {
            var userRole = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("PhoneNumber", user.Email),
                new Claim("FullName", user.FullName ?? string.Empty)
            };

            foreach (var role in userRole)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var authSigningCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now, // Thời gian bắt đầu hiệu lực của token là ngay bây giờ
                expires: DateTime.Now.AddHours(1), // Thời gian hết hạn của token là 1 tiếng
                claims: authClaims, //danh sách thông tin của người dùng
                signingCredentials: authSigningCredentials //Thông tin xác thực chữ ký của token
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }

        public async Task<string> GenerateJwtRefreshTokenAsync(ApplicationUser user, bool rememberMe)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var authSigningCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var expiration = GetRefreshTokenExpiration(rememberMe);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.UtcNow, // Thời gian bắt đầu hiệu lực của token là ngay bây giờ
                expires: DateTime.UtcNow.Add(expiration), // Thời gian hết hạn của token là 1 
                claims: authClaims, //danh sách thông tin của người dùng
                signingCredentials: authSigningCredentials //Thông tin xác thực chữ ký của token
            );
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(token);

            return refreshToken;
        }

        public async Task<bool> StoreRefreshToken(string userId, string refreshToken, bool rememberMe)
        {
            string redisKey = $"userId:{userId}-refreshToken";
            var result = await _redisService.StoreKeyAsync(redisKey, refreshToken, GetRefreshTokenExpiration(rememberMe));

            return result;
        }

        private TimeSpan GetRefreshTokenExpiration(bool rememberMe)
        {
            var rememberMeDays = _configuration["JWT:RefreshTokenExpirationRememberMe"]; // Nếu RememberMe là true, token sẽ hết hạn dựa theo số ngày được cấu hình trong appsettings.json
            var shortHours = _configuration["JWT:RefreshTokenExpiration"];// Nếu RememberMe là false, token sẽ hết hạn dựa theo số giờ được cấu hình trong appsettings.json

            return rememberMe
                ? TimeSpan.FromDays(int.Parse(rememberMeDays ?? "7")) // Nếu RememberMe là true, token sẽ hết hạn mặc định là 7 ngày
                : TimeSpan.FromHours(int.Parse(shortHours ?? "3")); // Nếu RememberMe là false, token sẽ hết hạn mặc định là 3 giờ
        }
    }
}
