using DataLayer.Context;
using DNTCommon.Web.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;

namespace Services
{
    public static class ServiceCollection
    {
        public static void AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddDNTCommonWeb();
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                        serverDbContextOptionsBuilder =>
                        {
                            var minutes = (int)TimeSpan.FromMinutes(3).TotalSeconds;
                            serverDbContextOptionsBuilder.CommandTimeout(minutes);
                            serverDbContextOptionsBuilder.EnableRetryOnFailure();
                        });
            });

            AddServices(services);
        }
        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICaptchaService, CaptchaService>();

            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IEducationLevelsService, EducationLevelsService>();
            services.AddScoped<ICookieValidatorService, CookieValidatorService>();
            services.AddScoped<IAntiForgeryCookieService, AntiForgeryCookieService>();
            services.AddScoped<IUnitOfWork>(conf => conf.GetService<ApplicationDbContext>());

            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<IDbInitializerService, DbInitializerService>();
            services.AddScoped<ITokenStoreService, TokenStoreService>();
            services.AddScoped<ITokenValidatorService, TokenValidatorService>();
            services.AddScoped<ITokenFactoryService, TokenFactoryService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IEmailSender, EmailSender>(); ;
            services.AddScoped<ICommonService, CommonService>();
            services.AddScoped<IExcelReportService, ExcelReportService>(); ;
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IAuditService, AuditService>();
            //services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISelect2SuggestionService, Select2SuggestionService>();
            services.AddScoped<IContextMenuColumnsService, ContextMenuColumnsService>();

            services.AddScoped<IUserAccessCodeService, UserAccessCodeService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddScoped<IUserEducationService, UserEducationService>();
            services.AddScoped<IUserJobService, UserJobService>();
            services.AddScoped<IUserLanguageService, UserLanguageService>();
            services.AddScoped<IUserSoftWareSkillsService, UserSoftWareSkillsService>();

            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILanguageLevelService, LanguageLevelService>();
            services.AddScoped<IJobGroupsService, JobGroupsService>();
            services.AddScoped<IJobPositionsService, JobPositionsService>();
            services.AddScoped<ISoftWareSkillsService, SoftWareSkillsService>();
            services.AddScoped<IUserPriorityService, UserPriorityService>();
            services.AddScoped<IUserRecommendedService, UserRecommendedService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IJobRequestService, JobRequestService>();
            services.AddScoped<IUserPinJobsService, UserPinJobsService>();
            services.AddScoped<IDashboardService, DashboardService>();
            
        }
    }
}
