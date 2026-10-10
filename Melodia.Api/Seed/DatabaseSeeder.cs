using MediatR;
using Melodia.Application.Abstractions.Authentication;
using Melodia.Application.Features.Auth.Register;
using Melodia.Persistence;
using Melodia.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Melodia.Api.Seed;

internal static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<WebApplication>>();

        // 1. Bekleyen migration'ları uygula
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        // 2. Admin rolü
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        if (!await roleManager.RoleExistsAsync(Roles.Admin))
            await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));

        // 3. Admin kullanıcı (bilgiler User Secrets'tan)
        var email = app.Configuration["Admin:Email"];
        var password = app.Configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Admin bilgileri ayarlarda yok; admin kullanıcı oluşturulmadı.");
            return;
        }

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var admin = await userManager.FindByEmailAsync(email);

        if (admin is null)
        {
            // Normal kayıt akışıyla: Identity kullanıcısı + Listener, tek transaction
            var sender = services.GetRequiredService<ISender>();
            var result = await sender.Send(new RegisterCommand(email, password, "Admin"));

            if (result.IsFailure)
            {
                logger.LogError("Admin oluşturulamadı: {ErrorCode} {Description}", result.Error.Code, result.Error.Description);
                return;
            }

            admin = await userManager.FindByIdAsync(result.Value.ToString());
        }

        if (admin is not null && !await userManager.IsInRoleAsync(admin, Roles.Admin))
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
            logger.LogInformation("Admin kullanıcı hazır: {Email}", email);
        }
    }
}