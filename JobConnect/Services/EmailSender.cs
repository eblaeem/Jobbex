using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using ViewModel;

namespace Services
{
    public interface IEmailSender
    {
        Task Send(EmailSenderModel model);
    }
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfig _emailConfig;
        private readonly IViewRenderService _viewRenderService;
        private readonly ILogService _loggerService;

        public EmailSender(IOptionsSnapshot<EmailConfig> options,
            IViewRenderService viewRenderService,
            ILogService logService)
        {
            _emailConfig = options.Value;
            _viewRenderService = viewRenderService;
            _loggerService = logService;
        }
        public async Task Send(EmailSenderModel request)
        {
            if (string.IsNullOrEmpty(request.Email) == true)
            {
                return;
            }

            var smtpClient = new SmtpClient(_emailConfig.Server)
            {
                Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password),
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = false,
            };
            var mail = new MailMessage
            {
                From = new MailAddress(_emailConfig.Username, request.Title),
                Subject = request.Title,
                IsBodyHtml = true,
            };
            mail.To.Add(new MailAddress(request.Email));
            var body = request.Body;

            if (string.IsNullOrEmpty(request.ViewPath) == false)
            {
                body = await _viewRenderService.RenderToStringAsync(request.ViewPath, request.Data);
            }
            mail.Body = body;
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                await _loggerService.Log(new LogModel()
                {
                    Message = ex.Message,
                    AccessToken = request.Token,
                    MethodeName = "EmailSender",
                    IsError = true
                });
            }
        }
    }

}
