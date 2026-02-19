
using CRM_Backend.Data;
using CRM_Backend.Data.Seed;
using CRM_Backend.Middlewares;
using CRM_Backend.Repositories.Implementations;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Security.Email;
using CRM_Backend.Security.Jwt;
using CRM_Backend.Services.Implementations;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Controllers
// --------------------------------------------------
builder.Services.AddControllers();

// --------------------------------------------------
// Swagger
// --------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM_Backend",
        Version = "v1",
        Description = "Authentication, Authorization and User Management API for CRM system",
        Contact = new OpenApiContact
        {
            Name = "CRM Backend Team",
            Email = "support@yourdomain.com"
        }
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --------------------------------------------------
// Database (PostgreSQL)
// --------------------------------------------------
builder.Services.AddDbContext<CrmAuthDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

// --------------------------------------------------
// API behavior
// --------------------------------------------------
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// --------------------------------------------------
// CORS (⚠ tighten in production)
// --------------------------------------------------
var allowedOrigins = builder.Configuration
    .GetSection("Frontend:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});





// --------------------------------------------------
// Configuration Bindings
// --------------------------------------------------
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("Smtp"));

// --------------------------------------------------
// Authentication (JWT)
// --------------------------------------------------
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // set true in prod
        options.SaveToken = true;
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!))
        };
    });

// --------------------------------------------------
// Authorization
// --------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ACCOUNT_ACTIVE",
        policy => policy.RequireClaim("account_status", "ACTIVE"));

    options.AddPolicy("PASSWORD_RESET_COMPLETED",
        policy => policy.Requirements.Add(
            new ForcePasswordResetRequirement()));
});

// --------------------------------------------------
// Dependency Injection
// --------------------------------------------------

// HttpClient for GeoLocation (timeout protected)
builder.Services.AddHttpClient<IGeoLocationService, GeoLocationService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(3);
});

// Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, ForcePasswordResetHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddSingleton<IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();

// Services
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IDomainService, DomainService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserVisibilityService, UserVisibilityService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAdminUserListService, AdminUserListService>();
builder.Services.AddScoped<IAdminUserDetailsService, AdminUserDetailsService>();
builder.Services.AddScoped<IAdminUserSecurityService, AdminUserSecurityService>();
builder.Services.AddScoped<IAdminUserAuditLogService, AdminUserAuditLogService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAccessService, AccessService>();
builder.Services.AddScoped<IBootstrapSeeder, BootstrapSeeder>();
builder.Services.AddScoped<IUserSelfService, UserSelfService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserPasswordRepository, UserPasswordRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserSecurityRepository, UserSecurityRepository>();
builder.Services.AddScoped<ILoginAttemptRepository, LoginAttemptRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDomainRepository, DomainRepository>();

// --------------------------------------------------
var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IBootstrapSeeder>();
    await seeder.SeedAsync();
}

// Exception middleware FIRST
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() ||
    builder.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("DefaultCors");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
