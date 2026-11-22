using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Uptrail_Backend.Common.Responses;
using Uptrail_Backend.Data;
using Uptrail_Backend.Helpers.Implementations;
using Uptrail_Backend.Helpers.Interfaces;
using Uptrail_Backend.Middlewares;
using Uptrail_Backend.Repositories.Implementations;
using Uptrail_Backend.Repositories.Interfaces;
using Uptrail_Backend.Services.Implementations;
using Uptrail_Backend.Services.Interfaces;

var envCandidates = new[]
{
    Path.Combine(AppContext.BaseDirectory, ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
    Path.Combine(Directory.GetCurrentDirectory(), "Uptrail_Backend", ".env"),
    Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? string.Empty, "Uptrail_Backend", ".env")
};

foreach (var envCandidate in envCandidates)
{
    if (!string.IsNullOrWhiteSpace(envCandidate) && File.Exists(envCandidate))
    {
        Env.Load(envCandidate);
    }
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in environment (.env).");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? Environment.GetEnvironmentVariable("Jwt__Key")
    ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Key' is missing in environment (.env).");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? Environment.GetEnvironmentVariable("Jwt__Issuer")
    ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Issuer' is missing in environment (.env).");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? Environment.GetEnvironmentVariable("Jwt__Audience")
    ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Audience' is missing in environment (.env).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IPlatformAdminRepository, PlatformAdminRepository>();
builder.Services.AddScoped<IPlatformAdminAuthRepository, PlatformAdminAuthRepository>();
builder.Services.AddScoped<IPlatformAdminService, PlatformAdminService>();
builder.Services.AddScoped<IPlatformAdminAuthService, PlatformAdminAuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularAdmin", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin)) return false;
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                return uri.Host == "localhost" || uri.Host == "127.0.0.1";
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var firstError = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault(msg => !string.IsNullOrWhiteSpace(msg))
            ?? "Validation failed. Please verify the submitted data.";

        return new BadRequestObjectResult(ApiResponseFactory.ErrorResponse<object?>(firstError));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAngularAdmin");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
