using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using LibraSmartAPI.Data;
using LibraSmartAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройка порта по умолчанию
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
});

// Добавляем Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Настройка базы данных SQLite
var dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    "LibraSmart",
    "librasmart.db"
);

// Создаем папку если её нет
var directory = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
{
    Directory.CreateDirectory(directory);
}

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Настройка JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? "LibraSmart_Super_Secret_Key_2024_Min32Characters!";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Регистрируем сервисы
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LibraSmart API",
        Version = "v1",
        Description = "API для библиотечной системы управления"
    });

    // Настройка JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

var app = builder.Build();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryContext>();
        context.Database.EnsureCreated();
        DatabaseInitializer.Initialize(context);
        Console.WriteLine("✓ База данных инициализирована успешно");
        Console.WriteLine($"✓ Путь к БД: {dbPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Ошибка инициализации БД: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraSmart API v1");
        c.RoutePrefix = "api/docs";
    });
}

app.UseCors();

// Раздача статических файлов (Vue.js фронтенд)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback для Vue Router (SPA)
app.MapFallbackToFile("index.html");

Console.WriteLine("╔════════════════════════════════════════════════════════╗");
Console.WriteLine("║         LibraSmart - Библиотечная система             ║");
Console.WriteLine("╚════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine($"✓ Приложение запущено: http://localhost:5000");
Console.WriteLine($"✓ API Swagger: http://localhost:5000/api/docs");
Console.WriteLine($"✓ База данных: {dbPath}");
Console.WriteLine();
Console.WriteLine("Нажмите Ctrl+C для остановки...");

app.Run();
