using Api.Helper;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
var versionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = appName,
        Version = versionNumber.ToString()
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
       {
         new OpenApiSecurityScheme
         {
           Reference = new OpenApiReference
           {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
           }
         },
          new string[] { }
       }
  });
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddServicesExtensions(builder);

var app = builder.Build();

app.UseWebApplicationExtensions(builder);
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();

}
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
