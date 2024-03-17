using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public interface ISecurityService
    {
        string GetSha256Hash(string input);
        Guid CreateCryptographicallySecureGuid();
        bool HasUserAccess(int accessCodeId);
        string EncryptText(string input);
        string DecryptText(string input);
    }

    public class SecurityService : ISecurityService
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessCodeService _userAccessCodeService;
        private string Key = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public SecurityService(IMemoryCache memoryCache,
            IHttpContextAccessor httpContextAccessor,
            IUserAccessCodeService userAccessCodeService)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
            _userAccessCodeService = userAccessCodeService;
        }
        public string GetSha256Hash(string input)
        {
            using var hashAlgorithm = new SHA256CryptoServiceProvider();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
        public Guid CreateCryptographicallySecureGuid()
        {
            var bytes = new byte[16];
            _rand.GetBytes(bytes);
            return new Guid(bytes);
        }
        public bool HasUserAccess(int accessCodeId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }
            var mainAdmin = user.Identity.GetUserClaimValue("UserName");
            if (mainAdmin == "admin")
            {
                return true;
            }
            var userId = Convert.ToInt32(user.Identity.GetUserClaimValue("UserId"));
            var accessCodes = (List<int>)_memoryCache.Get(userId);
            if (accessCodes is null)
            {
                accessCodes = _userAccessCodeService.Get(userId).GetAwaiter().GetResult();
                if (accessCodes.Any())
                {
                    _memoryCache.Set(userId, accessCodes);
                }
                else
                {
                    return false;
                }
            }
            return accessCodes.Any(a => a == accessCodeId);
        }

        public string EncryptText(string input)
        {
            var bytesToEncrypted = Encoding.UTF8.GetBytes(input);
            var keyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
            var bytesEncrypted = AesEncrypt(bytesToEncrypted, keyBytes);
            var result = Convert.ToBase64String(bytesEncrypted);

            return Uri.EscapeDataString(result);
        }

        public string DecryptText(string input)
        {
            input = Uri.UnescapeDataString(input);

            var bytesToDecrypted = Convert.FromBase64String(input);
            var keyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Key));
            var bytesDecrypted = AesDecrypt(bytesToDecrypted, keyBytes);
            var result = Encoding.UTF8.GetString(bytesDecrypted);
            return result;
        }

        private byte[] AesEncrypt(byte[] bytesToEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using Aes aes = Aes.Create("AesManaged");
                aes.KeySize = 256;
                aes.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                using (var cs = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToEncrypted, 0, bytesToEncrypted.Length);
                    cs.Close();
                }
                encryptedBytes = memoryStream.ToArray();
            }
            return encryptedBytes;
        }
        private byte[] AesDecrypt(byte[] bytesToDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (var memoryStream = new MemoryStream())
            {
                using Aes aes = Aes.Create("AesManaged");
                aes.KeySize = 256;
                aes.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                using (var cs = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToDecrypted, 0, bytesToDecrypted.Length);
                    cs.Close();
                }
                decryptedBytes = memoryStream.ToArray();
            }
            return decryptedBytes;
        }
    }
}