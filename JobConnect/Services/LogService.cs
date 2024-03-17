using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ViewModel;
using ViewModel.Setting;

namespace Services
{
    public interface ILogService
    {
        Task Log(LogModel request);
    }
    public class LogService : ILogService
    {
        private readonly AppSettings _appSettings;
        private readonly IViewRendererService _viewRenderService;
        private readonly IHttpContextAccessor _contextAccessor;
        public LogService(IOptionsSnapshot<AppSettings> options,
            IViewRendererService viewRendererService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = options.Value;
            _viewRenderService = viewRendererService;
            _contextAccessor = httpContextAccessor;
        }
        public async Task Log(LogModel model)
        {
            var ipAddress = _contextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = _contextAccessor.HttpContext?.Connection?.LocalIpAddress?.ToString();
            }

            model.IpAddress = ipAddress ?? string.Empty;
            model.EventDateTime = DateTime.Now;
            model.ApplicationName = _appSettings.ApplicationName;

            var body = await _viewRenderService.RenderViewToStringAsync("_OutputMessanger", model);

            body = body.Trim();
            var color = "#32CD32";
            if (model.IsError == true)
            {
                color = "#FF2400";
            }
            if (string.IsNullOrEmpty(model.Color) == false)
            {
                color = model.Color;
            }
            var outputBody = new
            {
                room = _appSettings.Output,
                notify = 1,
                message = body,
                color = color
            };
            var data = new StringContent(JsonSerializer.Serialize(outputBody,
                new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }), Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("API-KEY", _appSettings.OutputApiKey);
            httpClient.BaseAddress = new Uri(_appSettings.OutputApiAddress);
            var outPutResponse = await httpClient.PostAsync("/api/notify", data, new CancellationToken());
        }
    }
}
