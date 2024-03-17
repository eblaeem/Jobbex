using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Api.Helper
{
    public interface IUserBlockCache
    {
        bool AddToCache(string key, int expriryTimeInSeconds);
        bool IsInCache(string key);
    }
    public class UserBlockCache : IUserBlockCache
    {
        private IMemoryCache _memoryCache;
        public UserBlockCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool AddToCache(string key, int expriryTimeInSeconds)
        {
            bool isSuccess = false;
            if (!IsInCache(key))
            {
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(expriryTimeInSeconds));

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .AddExpirationToken(new CancellationChangeToken(cancellationTokenSource.Token));

                _memoryCache.Set(key, DateTime.Now, cacheEntryOptions);

                isSuccess = true;
            }
            return isSuccess;
        }
        public bool IsInCache(string key)
        {
            var item = _memoryCache.Get(key);
            return item != null;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class BlockByIPAddressAttribute : ActionFilterAttribute
    {
        private IUserBlockCache _userBlockCache;
        public string Name { get; set; }
        public int Seconds { get; set; }
        public string Message { get; set; }
        public override void OnActionExecuting(ActionExecutingContext executingContext)
        {
            if (_userBlockCache == null)
            {
                var cache = executingContext.HttpContext.RequestServices.GetService(typeof(IUserBlockCache));
                _userBlockCache = (IUserBlockCache)cache;
            }

            var ipAddress = executingContext.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (ipAddress is null)
            {
                ipAddress = executingContext.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            var key = $"{Name}_{ipAddress}";

            var allowExecute = _userBlockCache.AddToCache(key, Seconds);
            if (!allowExecute)
            {
                if (string.IsNullOrEmpty(Message))
                    Message = $"این درخواست رو هر {Seconds} ثانیه  یکبار می توانید فراخوانی نمایید.";

                executingContext.Result = new ContentResult
                {
                    Content = Message,
                    StatusCode = (int)HttpStatusCode.Conflict
                };
            }
        }
    }
}
