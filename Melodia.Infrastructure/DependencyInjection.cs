using System.Text;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Abstractions.Notifications;
using Melodia.Application.Abstractions.Storage;
using Melodia.Infrastructure.Authentication;
using Melodia.Infrastructure.Notifications;
using Melodia.Infrastructure.Storage;
using Melodia.Persistence;
using Melodia.Persistence.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Melodia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddHttpContextAccessor();

        AddOptions(services, configuration);
        AddIdentity(services);
        AddJwtAuthentication(services);

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IStreamUrlSigner, StreamUrlSigner>();
        services.AddScoped<IAudioFileStorage, AudioFileStorage>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static void AddOptions(IServiceCollection services, IConfiguration configuration)
    {
        // Ayarlar eksik/hatalıysa uygulama açılışta durur (fail fast)
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer) && !string.IsNullOrWhiteSpace(o.Audience), "Jwt:Issuer ve Jwt:Audience zorunludur.")
            .Validate(o => o.SecretKey?.Length >= 32, "Jwt:SecretKey en az 32 karakter olmalıdır.")
            .ValidateOnStart();

        services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.AudioRootPath), "Storage:AudioRootPath zorunludur.")
            .Validate(o => o.StreamSigningKey?.Length >= 32, "Storage:StreamSigningKey en az 32 karakter olmalıdır.")
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host) && !string.IsNullOrWhiteSpace(o.FromAddress), "Email:Host ve Email:FromAddress zorunludur.")
            .ValidateOnStart();
    }

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;

            // RegisterCommandValidator ile birebir aynı kurallar
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = false;

            // Brute force koruması: 5 hatalı denemede 15 dakika kilit
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();
    }

    private static void AddJwtAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                
                bearer.MapInboundClaims = false;

                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = JwtProvider.RoleClaim
                };
            });

        services.AddAuthorization();
    }
}