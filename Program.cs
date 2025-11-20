using FluentValidation;
using Masters.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.File;
using System.Text;
using YardManagementApplication;
using YardManagementApplication.Helpers;
using YardManagementApplication.JWTTokenHandler;
using YardManagementApplication.Middlewares;
using YardManagementApplication.Models;
using YardManagementApplication.Services;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()

    // File for Debug and Info ONLY (no warnings or errors)
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug || e.Level == LogEventLevel.Information)
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "debug-info-.txt"),
            rollingInterval: RollingInterval.Day))

    // File for Errors and Fatal ONLY
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error || e.Level == LogEventLevel.Warning)
        .WriteTo.File(
            path: Path.Combine(AppContext.BaseDirectory, "logs", "errors-.txt"),
            rollingInterval: RollingInterval.Day))

    .CreateLogger();
try
{
    Log.Information("application started");
    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();



    var builder = WebApplication.CreateBuilder(args);
    //var config = builder.Configuration;

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(360);
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddScoped<IJwtService, JwtService>();
    // Add JwtAuthorizationMessageHandler to inject token
    builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

    // === Read ServerURL from appsettings.json ===
    //var serverUrl = config["ServerURL"]?.TrimEnd('/') + "/";

    var apiuri = config["Api:ServerURL"] ?? "http://localhost:8099/";

    // === Register OpenAPI client properly ===
    builder.Services.AddHttpClient<v1Client>(client =>
    {
        client.BaseAddress = new Uri(apiuri);

    })
    .AddHttpMessageHandler<JwtAuthorizationMessageHandler>()
    .AddTypedClient((httpClient, sp) =>
    {
        return new v1Client(httpClient)
        {
            BaseUrl = apiuri
        };
    });



    // === Validators === need to check
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.DepartmentModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.ShiftModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.VehicleModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.BreakTimeModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.VehicleBrandModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.ReasonModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.RouteModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.VehicleTypeModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.DeliveryOrderModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.EmployeeModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.AppUserModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.EolProductionModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.ReWorkModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.HolidayModel>();
    builder.Services.AddValidatorsFromAssemblyContaining<YardManagementApplication.Models.UserGroupModel>();

    builder.Services.AddScoped<CsvUploadService>();


    builder.Services.AddProblemDetails();
    builder.Services.Configure<JwtSettings>(config.GetSection("Jwt"));
    var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>();
    // === JWT Authentication ===
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
     .AddJwtBearer(options =>
     {
         options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = jwtSettings?.Issuer,
             ValidAudience = jwtSettings?.Audience,
             IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Convert.FromBase64String(jwtSettings?.Key!)),
             ClockSkew = TimeSpan.Zero // Optional: Eliminate clock skew
         };
         options.SaveToken = true;
         options.Events = new JwtBearerEvents();
         options.Events.OnMessageReceived = ctx =>
         {
             if (ctx.Request.Cookies.ContainsKey("X-AccessToken"))
             {
                 ctx.Token = ctx.Request.Cookies["X-AccessToken"];
             }
             return Task.CompletedTask;
         };
     }).AddCookie(opt =>
     {
         opt.Cookie.SameSite = SameSiteMode.Strict;
         opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
         opt.Cookie.IsEssential = true;
     });


    builder.Logging.ClearProviders();
    //builder.Logging.AddSerilog();
    builder.Host.UseSerilog();


    var app = builder.Build();

    // === Middleware Pipeline ===
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseMiddleware<ApiUnauthorizedInterceptorMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseRouting();
    app.UseSession();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Login}/{action=Login}/{id?}");
    await app.RunAsync();
    //app.Run();



}
catch (Exception ex)
{
    Log.Error(ex, "Error occured in startup");
    throw;
}
finally
{
    _ = Log.CloseAndFlushAsync();
}