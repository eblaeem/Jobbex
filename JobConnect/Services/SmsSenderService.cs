using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using ViewModel;
using ViewModel.Setting;
using ViewModel.Sms;

namespace Services
{
    public interface ISmsService
    {
        Task<ResponseBase> IsSendSms(User user, int recordId);
        Task<ResponseBase> ValidateSms(SmsSenderValidate request);
        Task<ResponseBase> SendAsync(SmsSenderModel model);
        Task<List<ResponseBase>> SendAllAsync(List<LabelValue> items, string message);
        Task<List<SmsOutBoxResponse>> Get(SmsFilter request);
        Task<List<LabelValue>> SmsOutboxTypes();
        Task Save(SmsSenderModel model, string provideId = "");

    }
    public class SmsService : ISmsService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<SmsOutBox> _smsOutBoxes;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _appSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommonService _commonService;

        public SmsService(IUnitOfWork uow,
            IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<AppSettings> appSettings,
            IServiceProvider serviceProvider,
            ICommonService commonService)
        {
            _uow = uow;
            _smsOutBoxes = _uow.Set<SmsOutBox>();
            _httpClientFactory = httpClientFactory;
            _appSettings = appSettings.Value;
            _serviceProvider = serviceProvider;
            _commonService = commonService;
        }
        public async Task<List<SmsOutBoxResponse>> Get(SmsFilter request)
        {
            var query = from sms in _smsOutBoxes
                        from user in _uow.Set<User>().Where(w => w.PhoneNumber == sms.MobileNumber).DefaultIfEmpty()
                        from smsOutBoxType in _uow.Set<SmsOutboxType>().Where(w => w.Id == sms.SmsOutBoxTypeId).DefaultIfEmpty()
                        select new SmsOutBoxResponse
                        {
                            DateTime = sms.DateTime,
                            Id = sms.Id,
                            Message = sms.Message,
                            MobileNumber = sms.MobileNumber,
                            Token = sms.Token,
                            UserId = user.Id,
                            UserName = user.Username,
                            NationalCode = sms.NationalCode != null ? sms.NationalCode : user.NationalCode,
                            DisplayName = sms.DisplayName != null ? sms.DisplayName : user.DisplayName,
                            ProviderId = sms.ProvideId,
                            SmsOutBoxTypeId = sms.SmsOutBoxTypeId,
                            SmsOutBoxTypeName = smsOutBoxType.Name
                        };
            var fromDate = request.FromDate.ToGregorianDateTime();
            var toDate = request.ToDate.ToGregorianDateTime();
            if (fromDate is not null)
            {
                query = query.Where(w => w.DateTime >= fromDate.Value.AddDays(-1));
            }
            if (toDate is not null)
            {
                query = query.Where(w => w.DateTime <= toDate.Value.AddDays(1));
            }
            if (string.IsNullOrEmpty(request.MobileNumber) == false)
            {
                query = query.Where(w => w.MobileNumber.Contains(request.MobileNumber));
            }
            if (string.IsNullOrEmpty(request.Message) == false)
            {
                query = query.Where(w => w.Message.Contains(request.Message));
            }

            if (string.IsNullOrEmpty(request.Username) == false)
            {
                query = query.Where(w => w.UserName == request.Username);
            }
            if (string.IsNullOrEmpty(request.NationalCode) == false)
            {
                query = query.Where(w => w.NationalCode == request.NationalCode);
            }

            if (string.IsNullOrEmpty(request.DisplayName) == false)
            {
                query = query.Where(w => w.DisplayName.Contains(request.DisplayName));
            }

            if (request.UserId > 0)
            {
                query = query.Where(w => w.UserId == request.UserId);
            }
            if (request.SmsOutboxTypeId > 0)
            {
                query = query.Where(w => w.SmsOutBoxTypeId == request.SmsOutboxTypeId);
            }
            query = query.OrderBy(request.Sort);

            var totalRowCount = await query.CountAsync();
            var response = query.AsNoTracking()
                .Skip(request.PageNumber * request.PageSize).Take(request.PageSize)
                .ToList();
            if (response.Any() == false)
            {
                return new List<SmsOutBoxResponse>();
            }
            response.FirstOrDefault().TotalRowCount = totalRowCount;
            foreach (var item in response)
            {
                item.DateTimeString = item.DateTime.ToShortPersianDateTimeString();
            }
            return response;
        }

        public async Task<ResponseBase> SendAsync(SmsSenderModel model)
        {
            if (string.IsNullOrEmpty(model.MobileNumber))
            {
                return new ResponseBase(false, "شماره همراه مورد نظر نمی تواند خالی باشد");
            }
            if (string.IsNullOrEmpty(model.Message))
            {
                return new ResponseBase(false, "متن نمی تواند خالی باشد");
            }

            if (model.MobileNumber.StartsWith("0") == true)
            {
                model.MobileNumber = _commonService.ReplcaeMobileNumber98(model.MobileNumber);
            }
            if (string.IsNullOrEmpty(model.DisplayName))
            {
                var query = await (from user in _uow.Set<User>().Where(w => w.PhoneNumber == model.MobileNumber).DefaultIfEmpty()
                                   select new
                                   {
                                       user.DisplayName,
                                       user.NationalCode
                                   }).FirstOrDefaultAsync();
                if (query != null)
                {
                    model.DisplayName = query.DisplayName;
                    model.NationalCode = query.NationalCode;
                }
            }

            switch (_appSettings.SmsProvider)
            {
                case "cando":
                    return await SendByCandoo(model);
                case "adpdigital":
                    return await SendByAdpdigital(model);
                case "asiaTech":
                    return await SendByAsiaTech(model);
            }
            return new ResponseBase(false, "پنلی برای ارسال اس ام اس مشخص نشده است");
        }
        public async Task<List<ResponseBase>> SendAllAsync(List<LabelValue> items, string message)
        {
            var result = new List<ResponseBase>();

            var mobiles = items.Select(ReplcaeMobileNumber98).ToList();
            var mobilesList = mobiles.Select(s => s.Label).ToList();
            var users = _uow.Set<User>().Where(w => mobilesList.Any(a => a == w.PhoneNumber))
                .Select(item => new
                {
                    item.DisplayName,
                    item.NationalCode,
                    item.PhoneNumber
                }).ToList();


            foreach (var item in mobiles)
            {
                ResponseBase response = null;
                try
                {
                    var user = users.FirstOrDefault(w => w.PhoneNumber == item.Label);
                    response = await SendAsync(new SmsSenderModel()
                    {
                        Message = message,
                        MobileNumber = item.Label,
                        RecordId = Convert.ToInt32(item.Value),
                        SmsNumber = _appSettings.SmsNumber,
                        DisplayName = user?.DisplayName,
                        NationalCode = user?.NationalCode,
                    });
                    result.Add(response);
                }
                catch (Exception ex)
                {
                    result.Add(new ResponseBase
                    {
                        IsValid = false,
                        Message = $"{ex.Message} {ex.InnerException} {response?.Message}",
                    });
                }
            }
            return result;
        }
        public async Task Save(SmsSenderModel model, string provideId = "")
        {
            _smsOutBoxes.Add(new SmsOutBox()
            {
                DateTime = DateTime.Now,
                Message = model.Message,
                MobileNumber = model.MobileNumber,
                RecordId = Convert.ToInt32(model.RecordId),
                Token = string.IsNullOrEmpty(model.Token) == false ? model.Token : " ",
                ProvideId = provideId,
                SmsNumber = _appSettings.SmsNumber,
                SmsOutBoxTypeId = model.SmsOutBoxTypeId,
                ErrorCode = model.ErrorCode,
                DisplayName = model.DisplayName,
                NationalCode = model.NationalCode
            });
            await _uow.SaveChangesAsync();
        }

        public async Task<ResponseBase> ValidateSms(SmsSenderValidate request)
        {
            var response = await _smsOutBoxes.FirstOrDefaultAsync(w => w.Token == request.Token && w.RecordId == request.RecordId);
            if (response is null)
            {
                return new ResponseBase(false, "اعتبار سنجی پیامک ناموفق می باشد");
            }
            if (response.DateTime.AddMinutes(2) <= DateTime.Now)
            {
                return new ResponseBase(false, "اعتبار پیامک منقضی شده است");
            }
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> IsSendSms(User user, int recordId)
        {
            var response = await _smsOutBoxes.Where(w => w.RecordId == recordId).OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();
            if (response is null)
            {
                return new ResponseBase(true);
            }
            if (response.MobileNumber == user.PhoneNumber && response.DateTime.AddMinutes(2) >= DateTime.Now)
            {
                return new ResponseBase(false, "توکن قبلی تا زمان معتبر میباشد" + response.DateTime.AddMinutes(2).ToShortPersianDateTimeString());
            }
            return new ResponseBase(true);
        }

        public async Task<List<LabelValue>> SmsOutboxTypes()
        {
            return await _uow.Set<SmsOutboxType>().Select(s => new LabelValue
            {
                Value = s.Id,
                Label = s.Name
            }).ToListAsync();
        }
        private async Task<ResponseBase> SendByCandoo(SmsSenderModel model)
        {
            var url = "https://my.candoosms.com/services/rest/index.php";
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
            using var httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromMinutes(60);

            var json = new
            {
                username = _appSettings.SmsUserName,
                password = _appSettings.SmsPassword,
                method = "send",
                messages = new List<CandoResponse>
                            {
                                new CandoResponse()
                                {
                                    Body =model.Message,
                                    Recipient=model.MobileNumber,
                                    Sender=_appSettings.SmsNumber,
                                    CustomerId=model.RecordId
                                }
                            }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(json))
            };
            var response = await httpClient.SendAsync(request);
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            var candoResult = JsonConvert.DeserializeObject<CandoResult>(result);
            if (candoResult is null)
            {
                model.ErrorCode = result;
                await Save(model);
                return new ResponseBase(false, result);
            }

            var firstData = candoResult.Data?.FirstOrDefault();
            if (string.IsNullOrEmpty(firstData.ErrorCode) == false)
            {
                await ErrorLog($"{candoResult.Code} {candoResult.Message} {firstData.ErrorCode}", model.MobileNumber, "Candoo");
                model.ErrorCode = firstData.ErrorCode;
                await Save(model);
                return new ResponseBase(false);
            }

            var providerId = candoResult.Data.FirstOrDefault()?.ServerId.ToString();
            await Save(model, providerId);
            return new ResponseBase(true, $"پیامک برای شماره همراه {model.MobileNumber} ارسال گردید");
        }
        private async Task<ResponseBase> SendByAdpdigital(SmsSenderModel model)
        {
            var apiAddress = $"http://ws.adpdigital.com/url/send" +
                $"?username={_appSettings.SmsUserName}" +
                $"&password={_appSettings.SmsPassword}" +
                $"&dstaddress={model.MobileNumber}" +
                $"&body={HttpUtility.UrlEncode(model.Message)}" +
                $"&unicode=1";

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, apiAddress));

            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode == false)
            {
                await ErrorLog($"{response.StatusCode}", model.MobileNumber, "Adpdigital");
                model.ErrorCode = result;
                await Save(model);

                return new ResponseBase(false, response.StatusCode.ToString());
            }

            result = result.Replace("\r", "").Replace("\n", "").Replace("--BEGIN", "").Replace("--END", "");
            var adpdigitalResponse = result.Split(":").Length > 0 ? result.Split(":")[1] : "";
            if (result.Contains("Err"))
            {
                await ErrorLog($"{adpdigitalResponse}", model.MobileNumber, "Adpdigital");

                model.ErrorCode = result;
                await Save(model);

                return new ResponseBase(false, adpdigitalResponse);
            }
            await Save(model, adpdigitalResponse);

            return new ResponseBase(true, $"پیامک برای شماره همراه {model.MobileNumber} ارسال گردید");
        }
        private async Task<ResponseBase> SendByAsiaTech(SmsSenderModel model)
        {
            var url = "https://smsapi.asiatech.ir/api/message/send";
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; },
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };

            using var httpClient = new HttpClient(clientHandler);
            httpClient.Timeout = TimeSpan.FromMinutes(60);

            var json = new List<AsiaTechResponse>{
                       new AsiaTechResponse
                        {
                                SourceAddress = _appSettings.SmsNumber,
                                DestinationAddress = model.MobileNumber,
                                MessageText = model.Message
                        }
             };

            var authenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_appSettings.SmsUserName}:{_appSettings.SmsPassword}"));
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(json))
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authenticationString);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request);
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode == false)
            {
                await ErrorLog("", model.MobileNumber, "AsiaTech");
                model.ErrorCode = result;
                await Save(model);

                return new ResponseBase(false);
            }

            var asiaTechResult = JsonConvert.DeserializeObject<AsiaTechResult>(result);
            if (asiaTechResult.Succeeded)
            {
                await Save(model, asiaTechResult.Data[0]);
                return new ResponseBase(true, $"پیامک برای شماره همراه {model.MobileNumber} ارسال گردید");

            }

            await ErrorLog(asiaTechResult.ResultCode.ToString(), model.MobileNumber, "AsiaTech");

            model.ErrorCode = asiaTechResult.ResultCode.ToString();
            await Save(model);
            return new ResponseBase(false, asiaTechResult.ResultCode.ToString());
        }

        private async Task ErrorLog(string error, string mobileNumber, string providerName)
        {
            var loggerService = _serviceProvider.GetRequiredService<ILogService>();
            await loggerService.Log(new LogModel()
            {
                Message = $"Sending {providerName} Sms Failure:{error}",
                AccessToken = $"PhoneNumber:{mobileNumber}",
                IsError = true
            });
        }

        private LabelValue ReplcaeMobileNumber98(LabelValue item)
        {
            if (item.Label.StartsWith("0") == true)
            {
                _commonService.ReplcaeMobileNumber98(item.Label);
                return item;
            }
            return item;
        }
    }
}
