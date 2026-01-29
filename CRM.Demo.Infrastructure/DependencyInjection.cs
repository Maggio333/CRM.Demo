using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CRM.Demo.Application.Common.Interfaces;
using CRM.Demo.Application.Customers.Interfaces;
using CRM.Demo.Application.Contacts.Interfaces;
using CRM.Demo.Application.Tasks.Interfaces;
using CRM.Demo.Application.Notes.Interfaces;
using CRM.Demo.Infrastructure.Persistence;
using CRM.Demo.Infrastructure.Persistence.Repositories;
using CRM.Demo.Infrastructure.Messaging;

namespace CRM.Demo.Infrastructure;

/// <summary>
/// Extension methods dla Dependency Injection.
/// Rejestruje wszystkie serwisy Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            ));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // MessageBus (Kafka Producer) - Singleton bo Producer powinien być jeden na aplikację
        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}
