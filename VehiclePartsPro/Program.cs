using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using VehiclePartsPro.Application.Interfaces;
using VehiclePartsPro.Application.Services;
using VehiclePartsPro.Domain.Entities;
using VehiclePartsPro.Infrastructure.Data;
using VehiclePartsPro.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region ───────────────────────── DATABASE ─────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region ───────────────────────── IDENTITY ─────────────────────────
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
#endregion

#region ───────────────────────── JWT AUTH ─────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key missing");

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

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),

        // IMPORTANT: ensures [Authorize(Roles="Admin")] works
        RoleClaimType = ClaimTypes.Role
    };
});
#endregion

#region ───────────────────────── AUTHORIZATION ─────────────────────────
builder.Services.AddAuthorization();
#endregion

#region ───────────────────────── DI SERVICES ─────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPartService, PartService>();
#endregion

#region ───────────────────────── CONTROLLERS + OPENAPI ─────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();
#endregion

#region ───────────────────────── CORS ─────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});
#endregion

var app = builder.Build();

#region ───────────────────────── MIDDLEWARE PIPELINE ─────────────────────────

// global exception handler (must be first)
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors();
app.UseHttpsRedirection();


app.UseAuthentication();   // reads JWT
app.UseAuthorization();    // enforces roles

app.MapControllers();

// OpenAPI endpoint
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");
}
#endregion

#region ───────────────────────── SEED ROLES + ADMIN ─────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // ---- ROLES ----
    string[] roles = { "Admin", "Staff", "Customer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ---- ADMIN USER ----
    const string adminEmail = "admin@vehicleparts.com";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
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
#endregion

app.Run();