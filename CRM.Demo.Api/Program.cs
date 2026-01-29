using Microsoft.EntityFrameworkCore;
using CRM.Demo.Application;
using CRM.Demo.Infrastructure;
using CRM.Demo.Infrastructure.Persistence;
using CRM.Demo.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Wyłącz HTTPS w Dockerze (brak certyfikatu)
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(80); // Tylko HTTP
    });
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CRM Demo API",
        Version = "v1",
        Description = "API dla systemu CRM - Modular Monolith z DDD i CQRS",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CRM Demo",
            Email = "demo@crm.com"
        }
    });
    
    // Włącz XML comments dla lepszej dokumentacji
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Application Layer - MediatR, AutoMapper, FluentValidation
builder.Services.AddApplication();

// Infrastructure Layer - DbContext, Repositories, UnitOfWork, MessageBus
builder.Services.AddInfrastructure(builder.Configuration);

// CORS (dla frontendu)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:5174")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Automatyczne zastosowanie migracji przy starcie (tylko w Dockerze/Development)
if (app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Czekaj na bazę danych (retry logic)
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(2);
            var retryCount = 0;
            var connected = false;

            while (retryCount < maxRetries && !connected)
            {
                try
                {
                    // Test połączenia
                    if (context.Database.CanConnect())
                    {
                        connected = true;
                        app.Logger.LogInformation("✅ Połączenie z bazą danych nawiązane");
                    }
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount < maxRetries)
                    {
                        app.Logger.LogWarning("⚠️ Baza danych nie jest jeszcze gotowa, ponawianie za {Delay}s (próba {Retry}/{MaxRetries})...", delay.TotalSeconds, retryCount, maxRetries);
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        app.Logger.LogError(ex, "❌ Nie można połączyć się z bazą danych po {MaxRetries} próbach", maxRetries);
                        throw;
                    }
                }
            }

            // Zastosuj migracje
            if (connected)
            {
                app.Logger.LogInformation("🔄 Stosowanie migracji bazy danych...");
                context.Database.Migrate();
                app.Logger.LogInformation("✅ Migracje zastosowane pomyślnie");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "❌ Błąd podczas stosowania migracji");
            // Nie przerywamy startu aplikacji - może baza nie jest jeszcze gotowa
            // W produkcji migracje powinny być uruchamiane ręcznie lub przez CI/CD
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS musi być PRZED UseHttpsRedirection i UseAuthorization
app.UseCors("AllowAll");

// Wyłącz HTTPS redirection w development (dla łatwiejszego testowania z frontendem)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Global Exception Handler
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Dispose MessageBus (Kafka Producer) przy zamykaniu aplikacji
var messageBus = app.Services.GetRequiredService<CRM.Demo.Application.Common.Interfaces.IMessageBus>();
app.Lifetime.ApplicationStopping.Register(() =>
{
    if (messageBus is IDisposable disposable)
    {
        disposable.Dispose();
    }
});

app.Run();
