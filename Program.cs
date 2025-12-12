using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =======================
// CONFIG (set your Supabase URL)
// =======================
var SUPABASE_URL = "https://YOUR_PROJECT_REF.supabase.co"; // <-- غيّرها
var VERCEL_FRONTEND = "https://almustashar-frontend-hznb.vercel.app";

// =======================
// Services
// =======================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Almustashar_app",
        Version = "v1"
    });

    // Allow Authorization header in Swagger
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

// ===== CORS for Vercel Frontend =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelFrontend", policy =>
    {
        policy
            .WithOrigins(VERCEL_FRONTEND)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =======================
// Auth (JWT from Supabase)
// =======================
//
// Supabase GoTrue issuer عادة يكون:
// https://<project-ref>.supabase.co/auth/v1
//
// Audience الافتراضي: authenticated
//
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{SUPABASE_URL}/auth/v1";
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{SUPABASE_URL}/auth/v1",

            ValidateAudience = true,
            ValidAudience = "authenticated",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            // This helps if you later store role claim names differently
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization(options =>
{
    // مبدئيًا: AdminOnly يعتمد على Claim اسمه role = "admin"
    // إذا استخدمت جدول profiles بدل Claim، نبدله بسياسة تتحقق من DB.
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

var app = builder.Build();

// =======================
// Middleware
// =======================

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Almustashar_app v1");
});

// CORS must be before auth
app.UseCors("VercelFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Root endpoint (اختياري)
app.MapGet("/", () => "Almustashar API is running successfully");

app.Run();
