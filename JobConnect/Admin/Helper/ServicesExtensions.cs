using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Services;
using Services.JsonConvertSerializer;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using ViewModel;
using ViewModel.Setting;
using ViewModel.User;
using Web.Services;

namespace Admin.Helper
{
    public static class ServicesExtensions
    {
        public static void AddServicesExtensions(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddCustomOptions(builder.Configuration);
            services.AddCommonServices(builder.Configuration);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(name: "AutomationPermission",
                    configurePolicy: policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.Requirements.Add(new AutomationPermissionRequirement());
                    });
            });
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.SlidingExpiration = true;
                options.LoginPath = "/user/login";
                options.LogoutPath = "/user/logout";
                options.AccessDeniedPath = new PathString("/accessCode");
                options.Cookie.Name = ".jobbex.cookie";
                options.Cookie.HttpOnly = true;
                //options.Cookie.SecurePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        var cookieValidatorService = context.HttpContext.RequestServices.GetRequiredService<ICookieValidatorService>();
                        return cookieValidatorService.ValidateAsync(context);
                    }
                };
            });

            services.AddMvc()
                .AddFluentValidation(options =>
                {
                    options.LocalizationEnabled = true;
                    options.ValidatorOptions.LanguageManager.Culture = new CultureInfo("fa");
                    options.RegisterValidatorsFromAssemblyContaining<LoginValidator>();
                    options.ValidatorOptions.DisplayNameResolver = (type, member, expression) =>
                    {
                        if (member != null)
                        {
                            var name = string.Empty;
                            var displayMember = member.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                            if (displayMember != null)
                            {
                                name = displayMember.Name;
                                return name;
                            }
                            return name;
                        }
                        return null;
                    };
                });
            services.AddControllers().
                    AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                        options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
                        options.JsonSerializerOptions.Converters.Add(new NullableTimeSpanToStringConverter());
                        options.JsonSerializerOptions.Converters.Add(new StringToDecimalConverter());
                    })
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddRazorRuntimeCompilation();
        }
        public static void UseWebApplicationExtensions(this WebApplication app, WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }

        private static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = 52428800;
            });
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddOptions<BearerTokensOptions>()
                                .Bind(configuration.GetSection("BearerTokens"))
                                .Validate(bearerTokens =>
                                {
                                    return bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes;
                                }, "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
            services.AddOptions<AppSettings>()
                    .Bind(configuration.GetSection("AppSettings"));

            services.AddOptions<EmailConfig>()
                    .Bind(configuration.GetSection("Smtp"));
        }
    }
}
