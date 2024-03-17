using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ViewModel;
using ViewModel.User;

namespace Services
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(AddUserTokenRequest request);
        Task<bool> IsValidTokenAsync(string accessToken, int userId);
        Task<UserToken> FindTokenAsync(string refreshTokenValue);
        Task DeleteTokenAsync(string refreshTokenValue);
        Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue);
    }

    public class TokenStoreService : ITokenStoreService
    {
        private readonly ISecurityService _securityService;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserToken> _tokens;
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly ITokenFactoryService _tokenFactoryService;

        public TokenStoreService(
            IUnitOfWork uow,
            ISecurityService securityService,
            IOptionsSnapshot<BearerTokensOptions> configuration,
            ITokenFactoryService tokenFactoryService)
        {
            _uow = uow;
            _securityService = securityService;
            _tokens = _uow.Set<UserToken>();
            _configuration = configuration;
            _tokenFactoryService = tokenFactoryService;
        }

        public async Task AddUserTokenAsync(AddUserTokenRequest request)
        {
            var now = DateTimeOffset.UtcNow;
            await AddUserTokenAsync(new UserToken
            {
                UserId = request.UserId,
                RefreshTokenIdHash = _securityService.GetSha256Hash(request.RefreshTokenSerial),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(request.RefreshTokenSourceSerial) ?
                                           null : _securityService.GetSha256Hash(request.RefreshTokenSourceSerial),
                AccessTokenHash = _securityService.GetSha256Hash(request.AccessToken),
                RefreshTokenExpiresDateTime = now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                IpAddress = request.IpAddress,
                BrowserName = request.BrowserName,
            });
        }
        public async Task<bool> IsValidTokenAsync(string accessToken, int userId)
        {
            var accessTokenHash = _securityService.GetSha256Hash(accessToken);
            var userToken = await _tokens.FirstOrDefaultAsync(
                x => x.AccessTokenHash == accessTokenHash && x.UserId == userId);
            return userToken?.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
        }
        public Task<UserToken> FindTokenAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return Task.FromResult<UserToken>(null);
            }

            var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
            if (string.IsNullOrWhiteSpace(refreshTokenSerial))
            {
                return Task.FromResult<UserToken>(null);
            }

            var refreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial);
            return _tokens.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshTokenIdHash);
        }
        public async Task DeleteTokenAsync(string refreshTokenValue)
        {
            var token = await FindTokenAsync(refreshTokenValue);
            if (token != null)
            {
                _tokens.Remove(token);
            }
        }

        public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out int userId))
            {
                if (_configuration.Value.AllowSignoutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
                if (!string.IsNullOrWhiteSpace(refreshTokenSerial))
                {
                    var refreshTokenIdHashSource = _securityService.GetSha256Hash(refreshTokenSerial);
                    await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
                }
            }

            await DeleteExpiredTokensAsync();
        }

        private async Task AddUserTokenAsync(UserToken userToken)
        {
            if (!_configuration.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);
            _tokens.Add(userToken);
        }
        private async Task InvalidateUserTokensAsync(int userId)
        {
            await _tokens.Where(x => x.UserId == userId)
                        .ForEachAsync(userToken => _tokens.Remove(userToken));
        }
        private async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }
            await _tokens.Where(t => t.RefreshTokenIdHashSource == refreshTokenIdHashSource ||
                         t.RefreshTokenIdHash == refreshTokenIdHashSource &&
                          t.RefreshTokenIdHashSource == null)
            .ForEachAsync(userToken => _tokens.Remove(userToken));
        }
        private async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            await _tokens.Where(x => x.RefreshTokenExpiresDateTime < now)
                        .ForEachAsync(userToken => _tokens.Remove(userToken));
        }
    }
}