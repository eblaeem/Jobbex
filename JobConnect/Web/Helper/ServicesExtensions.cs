
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.JsonConvertSerializer;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ViewModel;
using ViewModel.Setting;
using ViewModel.User;

namespace Api.Helper
{
    public static class ServicesExtensions
    {
        public static void AddServicesExtensions(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddResponseCaching();
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"))
                .ProtectKeysWithDpapi();
            services.ConfigureProtected<SecretAppSetting>(builder.Configuration.GetSection("SecretAppSetting"));

            services.AddSingleton<IUserBlockCache, UserBlockCache>();
            services.AddCustomOptions(builder.Configuration);
        
            services.AddCommonServices(builder.Configuration);
            services.AddJwtBearer(builder.Configuration);
            //services.AddAntiforgery(options =>
            //{
            //    options.HeaderName = "X-XSRF-TOKEN";
            //    options.Cookie.Name = "X-XSRF-Token";
            //});

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
                        options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
                        options.JsonSerializerOptions.Converters.Add(new NullableTimeSpanToStringConverter());
                    })
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

            var allowedSites = new List<string>();
            builder.Configuration.Bind("AutomationCors:AllowedSites", allowedSites);
            services.AddCors(option =>
            {
                option.AddPolicy("AutomationCors", builder =>
                {
                    builder.SetIsOriginAllowed(isOriginAllowed: _ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(allowedSites.ToArray())
                            .AllowCredentials();
                });
            });
            services.AddSignalR();
        }
        public static void UseWebApplicationExtensions(this WebApplication app, WebApplicationBuilder builder)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("x-powered-by");
                context.Response.Headers.Remove("server");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                await next();
            });

            var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetService<IDbInitializerService>();
            if (dbInitializer != null)
            {
                dbInitializer.Initialize();
                dbInitializer.SeedData();
            }

            if (builder.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            var loggerService = scope.ServiceProvider.GetRequiredService<ILogService>();
            var userService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error?.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            State = 401,
                            Msg = "token expired"
                        }));
                    }
                    else if (error?.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        var message = error.Error.Message;
                        if (error.Error.InnerException != null)
                        {
                            message = error.Error.InnerException.Message;
                        }
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            State = 500,
                            Msg = message
                        }));

                        var token = userService.GetCurrentUserId().ToString();
                        await loggerService.Log(new LogModel()
                        {
                            Message = $"Api:{error.Error?.Message} {error.Error?.InnerException}",
                            StackTrace = $"{error.Error?.StackTrace} ",
                            AccessToken = $"UserId:{token}",
                            IsError = true
                        });
                    }
                    else
                    {
                        await next();
                    }
                });
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseCors("AutomationCors");
            app.UseResponseCaching();
            app.UseResponseCompression();
           

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void AddJwtBearer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(CustomRoles.Admin, policy => policy.RequireRole(CustomRoles.Admin));
                options.AddPolicy(CustomRoles.User, policy => policy.RequireRole(CustomRoles.User));
            });
            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["BearerTokens:Issuer"],
                    ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                    ValidAudience = configuration["BearerTokens:Audience"],
                    ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["BearerTokens:Key"])),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError("Authentication failed.", context.Exception);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                        return tokenValidatorService.ValidateAsync(context);
                    },
                    OnMessageReceived = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });
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
