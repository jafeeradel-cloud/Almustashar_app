using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
});

// ===== CORS for Vercel Frontend =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("VercelFrontend", policy =>
    {
        policy
            .WithOrigins("https://almustashar-frontend-hznb.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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

// CORS must be before Authorization & Controllers
app.UseCors("VercelFrontend");

app.UseAuthorization();

app.MapControllers();

// Optional: Root endpoint (prevents white page)
app.MapGet("/", () => "Almustashar API is running successfully");

app.Run();
