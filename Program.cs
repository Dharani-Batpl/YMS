using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YardManagementApplication;
using YardManagementApplication.Helpers;
using YardManagementApplication.JWTTokenHandler;
using YardManagementApplication.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(360);
});

builder.Services.AddHttpContextAccessor();

// Add JwtAuthorizationMessageHandler to inject token
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

// === Read ServerURL from appsettings.json ===
var serverUrl = config["ServerURL"]?.TrimEnd('/') + "/";

// === Register OpenAPI client properly ===
builder.Services.AddHttpClient<v1Client>(client =>
{
    client.BaseAddress = new Uri(serverUrl);
})
.AddHttpMessageHandler<JwtAuthorizationMessageHandler>()
.AddTypedClient((httpClient, sp) =>
{
    return new v1Client(httpClient)
    {
        BaseUrl = serverUrl
    };
});

// === Validators ===
builder.Services.AddValidatorsFromAssemblyContaining<DepartmentModel>();
builder.Services.AddValidatorsFromAssemblyContaining<ShiftModel>();
builder.Services.AddValidatorsFromAssemblyContaining<VehicleModel>();
builder.Services.AddValidatorsFromAssemblyContaining<BreakTimeModel>();
builder.Services.AddValidatorsFromAssemblyContaining<VehicleBrandModel>();
builder.Services.AddValidatorsFromAssemblyContaining<ReasonModel>();
builder.Services.AddValidatorsFromAssemblyContaining<RouteModel>();
builder.Services.AddValidatorsFromAssemblyContaining<VehicleTypeModel>();
builder.Services.AddValidatorsFromAssemblyContaining<DeliveryOrderModel>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeModel>();
builder.Services.AddValidatorsFromAssemblyContaining<AppUserModel>();
builder.Services.AddValidatorsFromAssemblyContaining<EolProductionModel>();
builder.Services.AddValidatorsFromAssemblyContaining<ReWorkModel>();
builder.Services.AddValidatorsFromAssemblyContaining<HolidayModel>();
builder.Services.AddValidatorsFromAssemblyContaining<UserGroupModel>();

builder.Services.AddScoped<CsvUploadService>();

// === JWT Authentication ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer missing"),
        ValidAudience = config["Jwt:Audience"] ?? throw new Exception("Jwt:Audience missing"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new Exception("Jwt:Key missing")))
    };
});

var app = builder.Build();

// === Middleware Pipeline ===
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<ApiUnauthorizedInterceptorMiddleware>();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();



//test
//test2
//test