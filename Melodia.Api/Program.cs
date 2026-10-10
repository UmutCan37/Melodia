using System.Threading.RateLimiting;
using Melodia.Api.Infrastructure;
using Melodia.Api.Seed;
using Melodia.Application;
using Melodia.Infrastructure;
using Melodia.Persistence;
using Melodia.Presentation;
using Melodia.Presentation.Endpoints;
using Scalar.AspNetCore;
using Serilog;

// 1. Uygulama ayaða kalkmadan önceki hatalarý da yakalayan geçici logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // 2. Asýl Serilog: ayarlarý appsettings'ten okur
    builder.Services.AddSerilog((services, logger) => logger
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    // 3. Katmanlar
    builder.Services
        .AddApplication()
        .AddPersistence(builder.Configuration)
        .AddInfrastructure(builder.Configuration);

    // 4. Hata yönetimi: beklenmeyen hatalar ? ProblemDetails
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // 5. API dokümantasyonu
    builder.Services.AddOpenApi();

    // 6. Angular'ýn API'ye eriþebilmesi
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

    // 7. Auth endpoint'lerine IP baþýna dakikada 10 istek
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddPolicy(AuthEndpoints.RateLimitPolicy, httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }));
    });

    var app = builder.Build();

    // --- Middleware sýrasý önemli ---
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
        await app.SeedDatabaseAsync();
    }

    app.UseCors();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapPresentationEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Uygulama baþlatýlamadý.");
}
finally
{
    Log.CloseAndFlush();
}