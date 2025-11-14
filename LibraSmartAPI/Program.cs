using Microsoft.EntityFrameworkCore;
using LibraSmartAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Настройка порта
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
});

// Добавляем Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep original casing
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

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "LibraSmart API",
        Version = "v1",
        Description = "Full-stack library management system API"
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
        Console.WriteLine("Database initialized successfully");
        Console.WriteLine($"Database path: {dbPath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization error: {ex.Message}");
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

// Serve static files (Vue.js frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Fallback for Vue Router (SPA)
app.MapFallbackToFile("index.html");

Console.WriteLine("============================================================");
Console.WriteLine("         LibraSmart - Library Management System            ");
Console.WriteLine("============================================================");
Console.WriteLine();
Console.WriteLine($"Application running: http://localhost:5000");
Console.WriteLine($"API docs: http://localhost:5000/api/docs");
Console.WriteLine($"Database: {dbPath}");
Console.WriteLine();
Console.WriteLine("Test credentials:");
Console.WriteLine("  Reader: alekseev@mail.ru / reader123");
Console.WriteLine("  Staff: petrova@library.ru / admin123");
Console.WriteLine();
Console.WriteLine("Press Ctrl+C to stop...");

app.Run();
