using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ReproIVF.Shared.Security;
using ReproIVF.Shared.Entities;
using ReproIVF.WebAPI.Security;
using ProgramEntity = ReproIVF.Shared.Entities.Program;

namespace ReproIVF.WebAPI.Data.Seed;

public static class SeedData
{
    public static async Task EnsureSeededAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("SeedData");

        var seedPath = Path.Combine(env.ContentRootPath, "Data", "Seed");
        await db.Database.MigrateAsync();

        if (!db.Programs.Any())
        {
            var programs = await LoadSeedAsync<SeedProgram>(seedPath, "programs.json", logger);
            var defaults = new[]
            {
                "ACCELERATE", "CET", "DISCOVERY", "GENERATE", "PRIVATE", "SLICK"
            };

            foreach (var name in defaults)
            {
                if (programs.All(p => !string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
                {
                    programs.Add(new SeedProgram { Name = name });
                }
            }

            db.Programs.AddRange(programs.Select(p => new ProgramEntity { Name = p.Name }));
        }

        if (!db.SemenTypes.Any())
        {
            db.SemenTypes.AddRange(
                new SemenType { Name = "conv" },
                new SemenType { Name = "sex" },
                new SemenType { Name = "Empty" }
            );
        }

        if (!db.PreservationMethods.Any())
        {
            db.PreservationMethods.AddRange(
                new PreservationMethod { Name = "Fresh" },
                new PreservationMethod { Name = "DT" },
                new PreservationMethod { Name = "Vit" }
            );
        }

        if (!db.Clients.Any())
        {
            var clients = await LoadSeedAsync<SeedClient>(seedPath, "clients.json", logger);
            db.Clients.AddRange(clients.Select(c => new Client { Name = c.Name }));
        }

        if (!db.Bulls.Any())
        {
            var bulls = await LoadSeedAsync<SeedBull>(seedPath, "bulls.json", logger);
            db.Bulls.AddRange(bulls.Select(b => new Bull { Name = b.Name, Code = b.Code }));
        }

        if (!db.Technicians.Any())
        {
            var technicians = await LoadSeedAsync<SeedTechnician>(seedPath, "technicians.json", logger);
            db.Technicians.AddRange(technicians.Select(t => new Technician { Name = t.Name }));
        }

        if (!db.Donors.Any())
        {
            var donors = await LoadSeedAsync<SeedDonor>(seedPath, "donors.json", logger);
            var programMap = await db.Programs.ToDictionaryAsync(p => p.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var donor in donors)
            {
                programMap.TryGetValue(donor.ProgramName ?? string.Empty, out var program);

                db.Donors.Add(new Donor
                {
                    Code = donor.Code,
                    OwnerName = donor.OwnerName,
                    ProgramId = program?.Id,
                    EmbryoCount = donor.EmbryoCount
                });
            }
        }

        if (!db.Users.Any())
        {
            var (adminHash, adminSalt) = PasswordHasher.HashPassword("Admin123!");
            var (clientHash, clientSalt) = PasswordHasher.HashPassword("Client123!");

            db.Users.AddRange(
                new AppUser
                {
                    Username = "admin",
                    Role = AppRoles.Admin,
                    PasswordHash = adminHash,
                    PasswordSalt = adminSalt
                },
                new AppUser
                {
                    Username = "client",
                    Role = AppRoles.Client,
                    PasswordHash = clientHash,
                    PasswordSalt = clientSalt
                }
            );
        }

        await db.SaveChangesAsync();
    }

    private static async Task<List<T>> LoadSeedAsync<T>(string seedPath, string fileName, ILogger logger) where T : class
    {
        var path = Path.Combine(seedPath, fileName);
        if (!File.Exists(path))
        {
            logger.LogWarning("Seed file not found: {Path}", path);
            return new List<T>();
        }

        var json = await File.ReadAllTextAsync(path);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind switch
            {
                JsonValueKind.Array => JsonSerializer.Deserialize<List<T>>(json, options) ?? new List<T>(),
                JsonValueKind.Object => JsonSerializer.Deserialize<T>(json, options) is { } item
                    ? new List<T> { item }
                    : new List<T>(),
                _ => new List<T>()
            };
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Invalid JSON in seed file: {Path}", path);
            return new List<T>();
        }
    }

    private class SeedProgram
    {
        public string Name { get; set; } = string.Empty;
    }

    private class SeedClient
    {
        public string Name { get; set; } = string.Empty;
    }

    private class SeedBull
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
    }

    private class SeedTechnician
    {
        public string Name { get; set; } = string.Empty;
    }

    private class SeedDonor
    {
        public string Code { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public string? ProgramName { get; set; }
        public int? EmbryoCount { get; set; }
    }
}
