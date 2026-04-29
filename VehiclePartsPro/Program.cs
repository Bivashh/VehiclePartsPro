using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VehiclePartsPro.Infrastructure.Data;
using VehiclePartsPro.Middleware;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Application.Services;
using VehiclePartsPro.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database (EF Core + Npgsql) ──────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── 2. Identity (setup) + (AddIdentityCore for APIs)
//
//   
//      "Use AddIdentityCore: If you are developing a Web API where you plan to use
//       token-based authentication... AddIdentityCore provides a minimal, flexible,
//       and cleaner setup for managing users."
//
//    AddIdentityCore gives us UserManager + RoleManager WITHOUT cookie auth,
//    which is what we want since we're doing JWT.
builder.Services.AddIdentityCore<User>(options =>
{   
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()                      // enables RoleManager<IdentityRole>
.AddEntityFrameworkStores<AppDbContext>()       // saves to our PostgreSQL via EF Core
.AddDefaultTokenProviders();                   // for password reset tokens etc.

// ── 3. JWT Authentication ────────────────────────────
//
//    (Authentication Scheme table):
//      "Bearer (JWT) → APIs, SPAs, mobile apps → Stateless, scalable, self-contained"
//
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing from appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// [Authorize], [Authorize(Roles="")], [AllowAnonymous]
builder.Services.AddAuthorization();

// ── 4. Application services —  (Dependency Injection, AddScoped) ───────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// ── 5. Controllers + OpenAPI ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ── 6. CORS — allows the frontend to call this API ────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────────
// "GlobalExceptionHandler should be added before all middlewares"
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();   // reads JWT from Authorization header — must come first
app.UseAuthorization();    // enforces [Authorize] attributes
app.MapControllers();

// ── Seed Admin user on first run ───────────────────────────────────────────────
// Seed roles are in AppDbContext.OnModelCreating (via migrations)
// The admin USER is seeded here at runtime so the password gets properly hashed by Identity
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    const string adminEmail = "admin@vehicleparts.com";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new User
        {
            FullName = "System Admin",
            Email = adminEmail,
            UserName = adminEmail
        };
        await userManager.CreateAsync(admin, "Admin@123");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

app.Run();