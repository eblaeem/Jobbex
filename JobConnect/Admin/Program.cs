using Admin.Helper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddServicesExtensions(builder);

var app = builder.Build();
app.UseWebApplicationExtensions(builder);
app.Run();
