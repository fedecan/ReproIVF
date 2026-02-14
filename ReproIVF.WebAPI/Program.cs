using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReproIVF.Shared.Security;
using ReproIVF.WebAPI.Data;
using ReproIVF.WebAPI.Data.Seed;
using ReproIVF.WebAPI.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy("AdminOrClient", policy => policy.RequireRole(AppRoles.Admin, AppRoles.Client));
});

builder.Services.AddScoped<JwtTokenService>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
var envOriginsRaw = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
var envOrigins = string.IsNullOrWhiteSpace(envOriginsRaw)
    ? Array.Empty<string>()
    : envOriginsRaw
        .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
var allowedOrigins = configuredOrigins
    .Concat(envOrigins)
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();
var exactOrigins = allowedOrigins
    .Where(origin => !origin.Contains('*') && Uri.TryCreate(origin, UriKind.Absolute, out _))
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
var wildcardOrigins = allowedOrigins
    .Where(origin => origin.Contains('*'))
    .ToArray();
var disableHttpsRedirection =
    builder.Configuration.GetValue<bool>("HttpsRedirection:Disable") ||
    string.Equals(
        Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECTION"),
        "true",
        StringComparison.OrdinalIgnoreCase);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
        policy.SetIsOriginAllowed(origin =>
            IsAllowedOrigin(origin, exactOrigins, wildcardOrigins))
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

await SeedData.EnsureSeededAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders();
if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static bool IsAllowedOrigin(string origin, HashSet<string> exactOrigins, string[] wildcardOrigins)
{
    var normalizedOrigin = origin.Trim().TrimEnd('/');
    if (exactOrigins.Contains(normalizedOrigin))
    {
        return true;
    }

    if (!Uri.TryCreate(normalizedOrigin, UriKind.Absolute, out var requestOrigin))
    {
        return false;
    }

    foreach (var wildcard in wildcardOrigins)
    {
        var pattern = wildcard.Trim().TrimEnd('/');
        const string wildcardMarker = "://*.";
        var markerIndex = pattern.IndexOf(wildcardMarker, StringComparison.Ordinal);
        if (markerIndex <= 0)
        {
            continue;
        }

        var patternScheme = pattern.Substring(0, markerIndex);
        var patternHostSuffix = pattern.Substring(markerIndex + wildcardMarker.Length);
        if (string.IsNullOrWhiteSpace(patternHostSuffix))
        {
            continue;
        }

        var schemeMatches = string.Equals(requestOrigin.Scheme, patternScheme, StringComparison.OrdinalIgnoreCase);
        var hostMatches = requestOrigin.Host.EndsWith("." + patternHostSuffix, StringComparison.OrdinalIgnoreCase);
        if (schemeMatches && hostMatches)
        {
            return true;
        }
    }

    return false;
}
